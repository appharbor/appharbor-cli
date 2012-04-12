using System;

namespace AppHarbor.Commands
{
	public class AddHostnameCommand : ICommand
	{
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly IAppHarborClient _appharborClient;

		public AddHostnameCommand(IApplicationConfiguration applicationConfiguration, IAppHarborClient appharborClient)
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
