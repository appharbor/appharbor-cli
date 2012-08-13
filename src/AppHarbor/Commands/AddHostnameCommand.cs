using System;

namespace AppHarbor.Commands
{
	[CommandHelp("Add a hostname", "[HOSTNAME]")]
	public class AddHostnameCommand : ApplicationCommand
	{
		private readonly IAppHarborClient _appharborClient;

		public AddHostnameCommand(IApplicationConfiguration applicationConfiguration, IAppHarborClient appharborClient)
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

			_appharborClient.CreateHostname(ApplicationId, arguments[0]);
		}
	}
}
