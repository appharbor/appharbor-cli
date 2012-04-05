using System;
using System.Collections.Generic;

namespace AppHarbor.Commands
{
	[CommandHelp("Display help summary")]
	public class HelpCommand : ICommand
	{
		private readonly IEnumerable<Type> _commandTypes;

		public HelpCommand(IEnumerable<Type> commandTypes)
		{
			_commandTypes = commandTypes;
		}

		public void Execute(string[] arguments)
		{
			throw new NotImplementedException();
		}
	}
}
