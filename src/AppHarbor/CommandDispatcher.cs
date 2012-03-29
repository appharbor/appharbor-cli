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
			var command = _commands.FirstOrDefault(x => x.GetType().Name.ToLower().StartsWith(commandName.ToLower()));

			if (command == null)
			{
				throw new ArgumentException(string.Format("The command \"{0}\" does not exist", commandName));
			}
			command.Execute(args.Skip(1).ToArray());
		}
	}
}
