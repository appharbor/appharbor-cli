using System;

namespace AppHarbor.Commands
{
	[CommandHelp("Add a hostname", additionalArgumentsHelp: "<HOSTNAME>", numberOfAdditionalArguments: 1)]
	public class AddHostnameCommand : ConsoleCommand
	{
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly IAppHarborClient _appharborClient;

		public AddHostnameCommand(IApplicationConfiguration applicationConfiguration, IAppHarborClient appharborClient)
		{
			_applicationConfiguration = applicationConfiguration;
			_appharborClient = appharborClient;
		}

		public override void Run(string[] arguments)
		{
			var applicationId = _applicationConfiguration.GetApplicationId();
			_appharborClient.CreateHostname(applicationId, arguments[0]);
		}
	}
}
