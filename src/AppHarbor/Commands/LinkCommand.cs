namespace AppHarbor.Commands
{
	public class LinkCommand : ICommand
	{
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly IAppHarborClient _appharborClient;

		public LinkCommand(IApplicationConfiguration applicationConfiguration, IAppHarborClient appharborClient)
		{
			_applicationConfiguration = applicationConfiguration;
			_appharborClient = appharborClient;
		}

		public void Execute(string[] arguments)
		{
			if (arguments.Length == 0)
			{
				throw new CommandException("Please specify an application id.");
			}

			var user = _appharborClient.GetUser();
			var application = _appharborClient.GetApplication(arguments[0]);

			_applicationConfiguration.SetupApplication(application.Slug, user);
		}
	}
}
