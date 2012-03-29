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
			var result = _appHarborApi.CreateApplication(arguments[0], arguments[1]);
			throw new NotImplementedException();
		}
	}
}
