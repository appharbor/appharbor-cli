namespace AppHarbor
{
	public class ApplicationConfiguration
	{
		private readonly IFileSystem _fileSystem;

		public ApplicationConfiguration(IFileSystem fileSystem)
		{
			_fileSystem = fileSystem;
		}
	}
}
