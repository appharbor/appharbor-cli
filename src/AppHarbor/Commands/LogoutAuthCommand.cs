using System;

namespace AppHarbor.Commands
{
	[CommandHelp("Logout of AppHarbor")]
	public class LogoutAuthCommand : ICommand
	{
		private readonly IAccessTokenConfiguration _accessTokenConfiguration;

		public LogoutAuthCommand(IAccessTokenConfiguration accessTokenConfiguration)
		{
			_accessTokenConfiguration = accessTokenConfiguration;
		}

		public void Execute(string[] arguments)
		{
			_accessTokenConfiguration.DeleteAccessToken();
		}
	}
}
