namespace AppHarbor.Commands
{
	[CommandHelp("Remove configuration variable", "[KEY1 KEY2..]")]
	public class RemoveConfigCommand : ApplicationCommand
	{
		private readonly IAppHarborClient _appharborClient;

		public RemoveConfigCommand(IApplicationConfiguration applicationConfiguration, IAppHarborClient appharborClient)
			: base(applicationConfiguration)
		{
			_appharborClient = appharborClient;
		}

		protected override void InnerExecute(string[] arguments)
		{
			if (arguments.Length == 0)
			{
				throw new CommandException("No configuration variable key was specified");
			}

			foreach (var key in arguments)
			{
				_appharborClient.RemoveConfigurationVariable(ApplicationId, key);
			}
		}
	}
}
