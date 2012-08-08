namespace AppHarbor.Commands
{
	[CommandHelp("Remove hostname from application", "[HOSTNAME]")]
	public class RemoveHostnameCommand : ICommand
	{
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly IAppHarborClient _appharborClient;

		public RemoveHostnameCommand(IApplicationConfiguration applicationConfiguration, IAppHarborClient appharborClient)
		{
			_applicationConfiguration = applicationConfiguration;
			_appharborClient = appharborClient;
		}

		public void Run(string[] arguments)
		{
			if (arguments.Length == 0)
			{
				throw new CommandException("No hostname was specified");
			}

			var applicationId = _applicationConfiguration.GetApplicationId();

			_appharborClient.RemoveHostname(applicationId, arguments[0]);
		}
	}
}
