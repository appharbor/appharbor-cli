using System;

namespace AppHarbor
{
	public class CommandException : Exception
	{
		public CommandException()
		{
		}

		public CommandException(string message)
			: base(message)
		{
		}
	}
}
