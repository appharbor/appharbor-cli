using System;
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

		public void Dispatch(string[] args)
		{
			throw new NotImplementedException();
		}
	}
}
