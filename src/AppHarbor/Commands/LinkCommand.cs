using AppHarbor.Model;

namespace AppHarbor.Commands
{
	[CommandHelp("Link this directory to an existing application", "[SLUG]", alias: "link")]
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
