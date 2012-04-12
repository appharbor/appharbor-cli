using System;

namespace AppHarbor.Commands
{
	class RemoveConfigCommand : ICommand
	{
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly IAppHarborClient _appharborClient;

		public RemoveConfigCommand(IApplicationConfiguration applicationConfiguration, IAppHarborClient appharborClient)
		{
			_applicationConfiguration = applicationConfiguration;
			_appharborClient = appharborClient;
		}

		public void Execute(string[] arguments)
		{
			throw new NotImplementedException();
		}
	}
}
