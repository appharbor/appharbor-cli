namespace AppHarbor.Commands
{
	[CommandHelp("Remove configuration variable", additionalArgumentsHelp: "[KEY1 KEY2..]", numberOfAdditionalArguments: 1)]
	public class RemoveConfigCommand : ConsoleCommand
	{
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly IAppHarborClient _appharborClient;

		public RemoveConfigCommand(IApplicationConfiguration applicationConfiguration, IAppHarborClient appharborClient)
		{
			_applicationConfiguration = applicationConfiguration;
			_appharborClient = appharborClient;
		}

		public override void Run(string[] arguments)
		{
			var applicationId = _applicationConfiguration.GetApplicationId();

			foreach (var key in arguments)
			{
				_appharborClient.RemoveConfigurationVariable(applicationId, key);
			}
		}
	}
}
