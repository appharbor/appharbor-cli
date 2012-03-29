using System;

namespace AppHarbor.Commands
{
	public class LoginCommand : ICommand
	{
		private readonly AccessTokenConfiguration _accessTokenConfiguration;

		public LoginCommand(AccessTokenConfiguration accessTokenConfiguration)
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
