using System;

namespace AppHarbor.Commands
{
	public class AuthCommand : ICommand
	{
		private readonly IAppHarborClient _appharborClient;

		public AuthCommand(IAppHarborClient appharborClient)
		{
			_appharborClient = appharborClient;
		}

		public void Execute(string[] arguments)
		{
			throw new NotImplementedException();
		}
	}
}
