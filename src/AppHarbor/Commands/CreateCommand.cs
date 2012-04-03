using System;

namespace AppHarbor.Commands
{
	public class CreateCommand : ICommand
	{
		private readonly IAppHarborClient _appHarborClient;
		private readonly ApplicationConfiguration _applicationConfiguration;

		public CreateCommand(IAppHarborClient appHarborClient, ApplicationConfiguration applicationConfiguration)
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
