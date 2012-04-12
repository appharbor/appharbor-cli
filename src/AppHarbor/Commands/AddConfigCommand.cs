namespace AppHarbor.Commands
{
	[CommandHelp("Add configuration variable to application", options: "[KEY=VALUE]")]
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
			if (arguments.Length == 0)
			{
				throw new CommandException("No configuration variables are specified");
			}
			var applicationId = _applicationConfiguration.GetApplicationId();

			foreach (var argument in arguments)
			{
				var splitted = argument.Split('=');
				_appharborClient.CreateConfigurationVariable(applicationId, splitted[0], splitted[1]);
			}
		}
	}
}
