using System;

namespace AppHarbor.Commands
{
	[CommandHelp("Unlink application from directory", alias: "unlink")]
	public class UnlinkAppCommand : ICommand
	{
		private readonly IApplicationConfiguration _applicationConfiguration;

		public UnlinkAppCommand(IApplicationConfiguration applicationConfiguration)
		{
			_applicationConfiguration = applicationConfiguration;
		}

		public void Execute(string[] arguments)
		{
			_applicationConfiguration.DeleteApplication();
		}
	}
}
