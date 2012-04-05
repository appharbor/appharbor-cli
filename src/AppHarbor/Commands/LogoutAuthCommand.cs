using System;

namespace AppHarbor.Commands
{
	[CommandHelp("Logout of AppHarbor")]
	public class LogoutAuthCommand : ICommand
	{
		private readonly AccessTokenConfiguration _accessTokenConfiguration;

		public LogoutAuthCommand(AccessTokenConfiguration accessTokenConfiguration)
		{
			_accessTokenConfiguration = accessTokenConfiguration;
		}

		public void Execute(string[] arguments)
		{
			_accessTokenConfiguration.DeleteAccessToken();
		}
	}
}
