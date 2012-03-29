using System;
using System.Linq;
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
			var commandName = args[0];
			var command = _commands.Single(x => x.GetType().Name.ToLower().StartsWith(commandName.ToLower()));

			command.Execute(args.Skip(1).ToArray());
		}
	}
}
