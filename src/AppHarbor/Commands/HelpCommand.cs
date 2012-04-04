using System;

namespace AppHarbor.Commands
{
	[CommandHelp("Display help summary or help for a specific command", "[COMMAND]")]
	public class HelpCommand : ICommand
	{
		public void Execute(string[] arguments)
		{
			throw new NotImplementedException();
		}
	}
}
