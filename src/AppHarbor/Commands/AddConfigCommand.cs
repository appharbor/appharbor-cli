namespace AppHarbor.Commands
{
	[CommandHelp("Add configuration variable to application", options: "[KEY=VALUE]")]
	public class AddConfigCommand : ApplicationCommand
	{
		private readonly IAppHarborClient _appharborClient;

		public AddConfigCommand(IApplicationConfiguration applicationConfiguration, IAppHarborClient appharborClient)
			: base(applicationConfiguration)
		{
			_appharborClient = appharborClient;
		}

		protected override void InnerExecute(string[] arguments)
		{
			if (arguments.Length == 0)
			{
				throw new CommandException("No configuration variables are specified");
			}

			foreach (var argument in arguments)
			{
				var splitted = argument.Split('=');
				_appharborClient.CreateConfigurationVariable(ApplicationId, splitted[0], splitted[1]);
			}
		}
	}
}
