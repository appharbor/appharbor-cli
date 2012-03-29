using System;

namespace AppHarbor.Commands
{
	public class LoginCommand : ICommand
	{
		private readonly AppHarborApi _appHarborApi;
		private readonly EnvironmentVariableConfiguration _environmentVariableConfiguration;

		public LoginCommand(AppHarborApi appHarborApi, EnvironmentVariableConfiguration environmentVariableConfiguration)
		{
			_appHarborApi = appHarborApi;
			_environmentVariableConfiguration = _environmentVariableConfiguration;
		}

		public void Execute(string[] arguments)
		{
			Console.WriteLine("Username:");
			var username = Console.ReadLine();

			Console.WriteLine("Password:");
			var password = Console.ReadLine();
		}
	}
}
