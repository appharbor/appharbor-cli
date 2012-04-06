using System;

namespace AppHarbor.Commands
{
	public class InfoAppCommand : ICommand
	{
		private readonly IAppHarborClient _client;

		public InfoAppCommand(IAppHarborClient client)
		{
			_client = client;
		}

		public void Execute(string[] arguments)
		{
			throw new NotImplementedException();
		}
	}
}
