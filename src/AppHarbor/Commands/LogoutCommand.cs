using System;

namespace AppHarbor.Commands
{
	[CommandHelp("Logout of AppHarbor")]
	public class LogoutCommand : ICommand
	{
		private readonly AccessTokenConfiguration _accessTokenConfiguration;

		public LogoutCommand(AccessTokenConfiguration accessTokenConfiguration)
		{
			_accessTokenConfiguration = accessTokenConfiguration;
		}

		public void Execute(string[] arguments)
		{
			_accessTokenConfiguration.DeleteAccessToken();
		}
	}
}
