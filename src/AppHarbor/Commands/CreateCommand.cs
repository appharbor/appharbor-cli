using System;

namespace AppHarbor.Commands
{
	public class CreateCommand : ICommand
	{
		private readonly AppHarborApi _appHarborApi;

		public CreateCommand(AppHarborApi appHarborApi)
		{
			_appHarborApi = appHarborApi;
		}

		public void Execute(string[] arguments)
		{
			throw new NotImplementedException();
		}
	}
}
