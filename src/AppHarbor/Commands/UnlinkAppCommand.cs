using System;

namespace AppHarbor.Commands
{
	[CommandHelp("Unlink application from directory", alias: "unlink")]
	public class UnlinkAppCommand : ICommand
	{
		public void Execute(string[] arguments)
		{
			throw new NotImplementedException();
		}
	}
}
