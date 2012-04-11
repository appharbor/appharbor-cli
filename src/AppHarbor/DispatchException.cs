using System;

namespace AppHarbor
{
	public class DispatchException : Exception
	{
		public DispatchException()
		{
		}

		public DispatchException(string message)
			: base(message)
		{
		}
	}
}
