using System;
using System.IO;
using System.Linq;
using AppHarbor.Model;

namespace AppHarbor
{
	public class GitRepositoryConfigurer : IGitRepositoryConfigurer
	{
		private readonly IFileSystem _fileSystem;
		private readonly IGitCommand _gitCommand;
		private readonly TextReader _reader;
		private readonly TextWriter _writer;

		public static string DefaultGitIgnore = "[Oo]bj\n[Bb]in\ndeploy\ndeploy/*\n*.csproj.user\n*.suo\n*.cache\npackages/\n";

		public GitRepositoryConfigurer(IFileSystem fileSystem, IGitCommand gitCommand, TextReader reader, TextWriter writer)
		{
			_fileSystem = fileSystem;
			_gitCommand = gitCommand;
			_reader = reader;
			_writer = writer;
		}

		public void Configure(string id, User user)
		{
			var repositoryUrl = string.Format("https://{0}@appharbor.com/{1}.git", user.Username, id);

			try
			{
				_gitCommand.Execute("--version");
			}
			catch (GitCommandException)
			{
				throw new RepositoryConfigurationException(string.Format("Git is not installed."));
			}

			try
			{
				_gitCommand.Execute("status");
			}
			catch (GitCommandException)
			{
				_writer.Write("Git repository is not initialized in this folder. Do you want to initialize it (type \"y\")?");
				if (_reader.ReadLine() != "y")
				{
					throw new RepositoryConfigurationException("Git repository was not initialized.");
				}

				_gitCommand.Execute("init");
				using (var stream = _fileSystem.OpenWrite(Path.Combine(Directory.GetCurrentDirectory(), ".gitignore")))
				{
					using (var writer = new StreamWriter(stream))
					{
						writer.Write(DefaultGitIgnore);
					}
				}
				_writer.WriteLine("Git repository was initialized with default .gitignore file.");
			}

			try
			{
				_gitCommand.Execute(string.Format("remote add appharbor {0}", repositoryUrl));

				_writer.WriteLine("Added \"appharbor\" as a remote repository. Push to AppHarbor with git push appharbor master.");
			}
			catch (GitCommandException)
			{
				throw new RepositoryConfigurationException(
					string.Format("Couldn't add appharbor repository as a git remote. Repository URL is: {0}.", repositoryUrl));
			}
		}

		public void Unconfigure()
		{
			try
			{
				_gitCommand.Execute("remote rm appharbor");
			}
			catch (GitCommandException)
			{
				throw new RepositoryConfigurationException("Git remote could not be removed.");
			}
		}

		public string GetApplicationId()
		{
			var url = _gitCommand.Execute("config remote.appharbor.url").FirstOrDefault();
			if (url == null)
			{
				throw new RepositoryConfigurationException();
			}

			return url.Split(new string[] { "/", ".git" }, StringSplitOptions.RemoveEmptyEntries).Last();
		}
	}
}
