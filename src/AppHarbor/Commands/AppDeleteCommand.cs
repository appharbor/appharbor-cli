using System;

namespace AppHarbor.Commands
{
	[CommandHelp("Delete application")]
	public class AppDeleteCommand : ICommand
	{
		public AppDeleteCommand()
		{
		}

		public void Execute(string[] arguments)
		{
			throw new NotImplementedException();
		}
	}
}
