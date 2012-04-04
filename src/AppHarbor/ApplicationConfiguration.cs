using System.IO;

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
				var stream = _fileSystem.OpenRead(appharborConfigurationFile.FullName);
				using (var reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}

			}
			catch (FileNotFoundException)
			{
				throw new ApplicationConfigurationException("Application is not configured");
			}
		}
	}
}
