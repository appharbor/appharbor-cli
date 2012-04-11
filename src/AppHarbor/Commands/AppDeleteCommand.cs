using System;

namespace AppHarbor.Commands
{
	[CommandHelp("Delete application")]
	public class AppDeleteCommand : ICommand
	{
		private readonly IAppHarborClient _appharborClient;

		public AppDeleteCommand(IAppHarborClient appharborClient)
		{
			_appharborClient = appharborClient;
		}

		public void Execute(string[] arguments)
		{
			throw new NotImplementedException();
		}
	}
}
