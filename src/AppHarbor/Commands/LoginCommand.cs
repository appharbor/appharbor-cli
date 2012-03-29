using System;

namespace AppHarbor.Commands
{
	public class LoginCommand : ICommand
	{
		private readonly AccessTokenFetcher _accessTokenFetcher;
		private readonly EnvironmentVariableConfiguration _environmentVariableConfiguration;

		public LoginCommand(AccessTokenFetcher accessTokenFetcher, EnvironmentVariableConfiguration environmentVariableConfiguration)
		{
			_accessTokenFetcher = accessTokenFetcher;
			_environmentVariableConfiguration = environmentVariableConfiguration;
		}

		public void Execute(string[] arguments)
		{
			if (_environmentVariableConfiguration.Get("AppHarborToken", EnvironmentVariableTarget.User) != null)
			{
				throw new CommandException("You're already logged in");
			}

			Console.WriteLine("Username:");
			var username = Console.ReadLine();

			Console.WriteLine("Password:");
			var password = Console.ReadLine();
		}
	}
}
