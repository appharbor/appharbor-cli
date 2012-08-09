namespace AppHarbor.Commands
{
	[CommandHelp("Remove hostname from application", additionalArgumentsHelp: "<HOSTNAME>", numberOfAdditionalArguments: 1)]
	public class RemoveHostnameCommand : ConsoleCommand
	{
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly IAppHarborClient _appharborClient;

		public RemoveHostnameCommand(IApplicationConfiguration applicationConfiguration, IAppHarborClient appharborClient)
		{
			_applicationConfiguration = applicationConfiguration;
			_appharborClient = appharborClient;
		}

		public override void Run(string[] arguments)
		{
			var applicationId = _applicationConfiguration.GetApplicationId();

			_appharborClient.RemoveHostname(applicationId, arguments[0]);
		}
	}
}
