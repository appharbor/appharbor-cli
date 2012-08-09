using System;

namespace AppHarbor
{
	public class ArgumentParsingException : Exception
	{
		public ArgumentParsingException(string message)
			: base(message)
		{
		}
	}
}
