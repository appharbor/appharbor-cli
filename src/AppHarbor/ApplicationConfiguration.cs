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
			var directory = Directory.GetCurrentDirectory();
			var appharborConfigurationFile = new FileInfo(Path.Combine(directory, ".appharbor"));

			try
			{
				using (var stream = _fileSystem.OpenRead(appharborConfigurationFile.FullName))
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
		}
	}
}
