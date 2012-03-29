using System.Collections.Generic;

namespace AppHarbor
{
	public class CommandDispatcher
	{
		private readonly IEnumerable<ICommand> _commands;

		public CommandDispatcher(IEnumerable<ICommand> commands)
		{
			_commands = commands;
		}
	}
}
