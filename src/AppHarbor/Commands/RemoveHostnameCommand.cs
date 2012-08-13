namespace AppHarbor.Commands
{
	[CommandHelp("Remove hostname from application", "[HOSTNAME]")]
	public class RemoveHostnameCommand : ApplicationCommand
	{
		private readonly IAppHarborClient _appharborClient;

		public RemoveHostnameCommand(IApplicationConfiguration applicationConfiguration, IAppHarborClient appharborClient)
			: base(applicationConfiguration)
		{
			_appharborClient = appharborClient;
		}

		protected override void InnerExecute(string[] arguments)
		{
			if (arguments.Length == 0)
			{
				throw new CommandException("No hostname was specified");
			}

			_appharborClient.RemoveHostname(ApplicationId, arguments[0]);
		}
	}
}
