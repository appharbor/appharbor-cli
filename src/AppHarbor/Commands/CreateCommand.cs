using System;

namespace AppHarbor.Commands
{
	public class CreateCommand : ICommand
	{
		private readonly AppHarborClient _appHarborClient;
		private readonly ApplicationConfiguration _applicationConfiguration;

		public CreateCommand(AppHarborClient appHarborClient, ApplicationConfiguration applicationConfiguration)
		{
			_appHarborClient = appHarborClient;
			_applicationConfiguration = applicationConfiguration;
		}

		public void Execute(string[] arguments)
		{
			_appHarborClient.CreateApplication(arguments[0], arguments[1]);
			throw new NotImplementedException();
		}
	}
}
