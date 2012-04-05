using System;

namespace AppHarbor.Commands
{
	[CommandHelp("Logout of AppHarbor")]
	public class AuthLogoutCommand : ICommand
	{
		private readonly AccessTokenConfiguration _accessTokenConfiguration;

		public AuthLogoutCommand(AccessTokenConfiguration accessTokenConfiguration)
		{
			_accessTokenConfiguration = accessTokenConfiguration;
		}

		public void Execute(string[] arguments)
		{
			_accessTokenConfiguration.DeleteAccessToken();
		}
	}
}
