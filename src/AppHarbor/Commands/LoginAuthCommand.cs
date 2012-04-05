using System;

namespace AppHarbor.Commands
{
	[CommandHelp("Login to AppHarbor")]
	public class LoginAuthCommand : ICommand
	{
		private readonly IAccessTokenConfiguration _accessTokenConfiguration;

		public LoginAuthCommand(IAccessTokenConfiguration accessTokenConfiguration)
		{
			_accessTokenConfiguration = accessTokenConfiguration;
		}

		public void Execute(string[] arguments)
		{
			if (_accessTokenConfiguration.GetAccessToken() != null)
			{
				throw new CommandException("You're already logged in");
			}

			Console.WriteLine("Username:");
			var username = Console.ReadLine();

			Console.WriteLine("Password:");
			var password = Console.ReadLine();

			_accessTokenConfiguration.SetAccessToken(username, password);
		}
	}
}
