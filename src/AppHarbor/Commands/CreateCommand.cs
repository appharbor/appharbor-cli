using System;

namespace AppHarbor.Commands
{
	public class CreateCommand : ICommand
	{
		private readonly AppHarborClient _appHarborClient;

		public CreateCommand(AppHarborClient appHarborClient)
		{
			_appHarborClient = appHarborClient;
		}

		public void Execute(string[] arguments)
		{
			_appHarborClient.CreateApplication(arguments[0], arguments[1]);
			throw new NotImplementedException();
		}
	}
}
