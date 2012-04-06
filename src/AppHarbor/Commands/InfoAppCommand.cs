using System;

namespace AppHarbor.Commands
{
	[CommandHelp("Get application details")]
	public class InfoAppCommand : ICommand
	{
		private readonly IAppHarborClient _client;
		private readonly IApplicationConfiguration _applicationConfiguration;

		public InfoAppCommand(IAppHarborClient client, IApplicationConfiguration applicationConfiguration)
		{
			_client = client;
			_applicationConfiguration = applicationConfiguration;
		}

		public void Execute(string[] arguments)
		{
			throw new NotImplementedException();
		}
	}
}
