using System;

namespace AppHarbor.Commands
{
	public class AppCommand : ICommand
	{
		private readonly IAppHarborClient _client;

		public AppCommand(IAppHarborClient appharborClient)
		{
			_client = appharborClient;
		}

		public void Execute(string[] arguments)
		{
			throw new NotImplementedException();
		}
	}
}
