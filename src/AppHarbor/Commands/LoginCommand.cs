using System;

namespace AppHarbor.Commands
{
	public class LoginCommand : ICommand
	{
		private readonly AppHarborApi _appHarborApi;

		public LoginCommand(AppHarborApi appHarborApi)
		{
			_appHarborApi = appHarborApi;
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
