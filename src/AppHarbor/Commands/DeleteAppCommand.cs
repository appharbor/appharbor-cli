namespace AppHarbor.Commands
{
	[CommandHelp("Delete application")]
	public class DeleteAppCommand : ApplicationCommand
	{
		private readonly IAppHarborClient _appharborClient;
		private readonly IApplicationConfiguration _applicationConfiguration;

		public DeleteAppCommand(IAppHarborClient appharborClient, IApplicationConfiguration applicationConfiguration)
			: base(applicationConfiguration)
		{
			_appharborClient = appharborClient;
			_applicationConfiguration = applicationConfiguration;
		}

		protected override void InnerExecute(string[] arguments)
		{
			_appharborClient.DeleteApplication(ApplicationId);

			_applicationConfiguration.RemoveConfiguration();
		}
	}
}
