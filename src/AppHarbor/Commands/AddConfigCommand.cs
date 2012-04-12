namespace AppHarbor.Commands
{
	public class AddConfigCommand : ICommand
	{
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly IAppHarborClient _appharborClient;

		public AddConfigCommand(IApplicationConfiguration applicationConfiguration, IAppHarborClient appharborClient)
		{
			_applicationConfiguration = applicationConfiguration;
			_appharborClient = appharborClient;
		}

		public void Execute(string[] arguments)
		{
			var applicationId = _applicationConfiguration.GetApplicationId();

			var splitted = arguments[0].Split('=');
			_appharborClient.CreateConfigurationVariable(applicationId, splitted[0], splitted[1]);
		}
	}
}
