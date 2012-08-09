using AppHarbor.Model;

namespace AppHarbor.Commands
{
	[CommandHelp("Link directory to an application", additionalArgumentsHelp: "<SLUG>", numberOfAdditionalArguments: 1, alias: "link")]
	public class LinkAppCommand : ConsoleCommand
	{
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly IAppHarborClient _appharborClient;

		public LinkAppCommand(IApplicationConfiguration applicationConfiguration, IAppHarborClient appharborClient)
		{
			_applicationConfiguration = applicationConfiguration;
			_appharborClient = appharborClient;
		}

		public override void Run(string[] arguments)
		{
			var user = _appharborClient.GetUser();

			Application application;
			try
			{
				application = _appharborClient.GetApplication(arguments[0]);
			}
			catch (ApiException)
			{
				throw new CommandException("The application could not be found");
			}

			_applicationConfiguration.SetupApplication(application.Slug, user);
		}
	}
}
