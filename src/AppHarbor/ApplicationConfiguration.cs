using System.IO;

namespace AppHarbor
{
	public class ApplicationConfiguration
	{
		private readonly IFileSystem _fileSystem;

		public ApplicationConfiguration(IFileSystem fileSystem)
		{
			_fileSystem = fileSystem;
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
