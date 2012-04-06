using System;
using System.IO;
using System.Linq;
using AppHarbor.Model;

namespace AppHarbor
{
	public class ApplicationConfiguration : IApplicationConfiguration
	{
		private readonly IFileSystem _fileSystem;
		private readonly IGitExecutor _gitExecutor;
		private readonly IGitRepositoryConfigurer _repositoryConfigurer;

		public ApplicationConfiguration(IFileSystem fileSystem, IGitExecutor gitExecutor, IGitRepositoryConfigurer repositoryConfigurer)
		{
			_fileSystem = fileSystem;
			_gitExecutor = gitExecutor;
			_repositoryConfigurer = repositoryConfigurer;
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
			}

			var url = _gitExecutor.Execute("config remote.appharbor.url", CurrentDirectory).FirstOrDefault();
			if (url != null)
			{
				return url.Split(new string[] { "/", ".git" }, StringSplitOptions.RemoveEmptyEntries).Last();
			}

			throw new ApplicationConfigurationException("Application is not configured");
		}

		public virtual void SetupApplication(string id, User user)
		{
			try
			{
				_repositoryConfigurer.Configure(id, user);
				return;
			}
			catch (RepositoryConfigurationException exception)
			{
				Console.WriteLine(exception.Message);
			}

			using (var stream = _fileSystem.OpenWrite(ConfigurationFile.FullName))
			{
				using (var writer = new StreamWriter(stream))
				{
					writer.Write(id);
				}
			}

			Console.WriteLine("Wrote application configuration to {0}", ConfigurationFile);
		}

		private static DirectoryInfo CurrentDirectory
		{
			get
			{
				return new DirectoryInfo(Directory.GetCurrentDirectory());
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
