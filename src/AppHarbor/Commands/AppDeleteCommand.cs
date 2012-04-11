namespace AppHarbor.Commands
{
	[CommandHelp("Delete application")]
	public class AppDeleteCommand : ICommand
	{
		private readonly IAppHarborClient _appharborClient;
		private readonly IApplicationConfiguration _applicationConfiguration;

		public AppDeleteCommand(IAppHarborClient appharborClient, IApplicationConfiguration applicationConfiguration)
		{
			_appharborClient = appharborClient;
			_applicationConfiguration = applicationConfiguration;
		}

		public void Execute(string[] arguments)
		{
			var id = _applicationConfiguration.GetApplicationId();
			_appharborClient.DeleteApplication(id);
		}
	}
}
