namespace AppHarbor.Commands
{
	[CommandHelp("Remove configuration variable", "[KEY1 KEY2..]")]
	public class RemoveConfigCommand : ICommand
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
			if (arguments.Length == 0)
			{
				throw new CommandException("No configuration variable key was specified");
			}

			var applicationId = _applicationConfiguration.GetApplicationId();

			foreach (var key in arguments)
			{
				_appharborClient.RemoveConfigurationVariable(applicationId, key);
			}
		}
	}
}
