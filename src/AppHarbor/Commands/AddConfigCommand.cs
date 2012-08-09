namespace AppHarbor.Commands
{
	[CommandHelp("Add configuration variable to application", additionalArgumentsHelp: "[KEY=VALUE]", numberOfAdditionalArguments: 1)]
	public class AddConfigCommand : ConsoleCommand
	{
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly IAppHarborClient _appharborClient;

		public AddConfigCommand(IApplicationConfiguration applicationConfiguration, IAppHarborClient appharborClient)
		{
			_applicationConfiguration = applicationConfiguration;
			_appharborClient = appharborClient;
		}

		public override void Run(string[] arguments)
		{
			var applicationId = _applicationConfiguration.GetApplicationId();

			foreach (var argument in arguments)
			{
				var splitted = argument.Split('=');
				_appharborClient.CreateConfigurationVariable(applicationId, splitted[0], splitted[1]);
			}
		}
	}
}
