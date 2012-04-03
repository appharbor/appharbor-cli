using System;

namespace AppHarbor.Commands
{
	public class CreateCommand : ICommand
	{
		private readonly AppHarborClient _appHarborClient;
		private readonly IFileSystem _fileSystem;

		public CreateCommand(AppHarborClient appHarborClient, IFileSystem fileSystem)
		{
			_appHarborClient = appHarborClient;
			_fileSystem = fileSystem;
		}

		public void Execute(string[] arguments)
		{
			_appHarborClient.CreateApplication(arguments[0], arguments[1]);
			throw new NotImplementedException();
		}
	}
}
