using System;
using System.IO;
using AppHarbor.Model;

namespace AppHarbor
{
	public class ApplicationConfiguration
	{
		private readonly IFileSystem _fileSystem;
		private readonly IGitExecutor _gitExecutor;

		public ApplicationConfiguration(IFileSystem fileSystem, IGitExecutor gitExecutor)
		{
			_fileSystem = fileSystem;
			_gitExecutor = gitExecutor;
		}

		public string GetApplicationId()
		{
			try
			{
				using (var stream = _fileSystem.OpenRead(ConfigurationFile.FullName))
				{
					using (var reader = new StreamReader(stream))
					{
						return reader.ReadToEnd();
					}
				}
			}
			catch (FileNotFoundException)
			{
				throw new ApplicationConfigurationException("Application is not configured");
			}
		}

		public void SetupApplication(string id, User user)
		{
			var repositoryUrl = string.Format("https://{0}@appharbor.com/{1}.git", user.Username, id);

			try
			{
				_gitExecutor.Execute(string.Format("remote add appharbor https://{0}@appharbor.com/{1}.git", user.Username, id),
					new DirectoryInfo(Directory.GetCurrentDirectory()));

				Console.WriteLine("Added \"appharbor\" as a remote repository. Push to AppHarbor with git push appharbor master");
				return;
			}
			catch (InvalidOperationException)
			{
			}

			Console.WriteLine("Couldn't add appharbor repository as a git remote. Repository URL is: {0}", repositoryUrl);

			using (var stream = _fileSystem.OpenWrite(ConfigurationFile.FullName))
			{
				using (var writer = new StreamWriter(stream))
				{
					writer.Write(id);
				}
			}
		}

		private static FileInfo ConfigurationFile
		{
			get
			{
				var directory = Directory.GetCurrentDirectory();
				var appharborConfigurationFile = new FileInfo(Path.Combine(directory, ".appharbor"));
				return appharborConfigurationFile;
			}
		}
	}
}
