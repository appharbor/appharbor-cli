using System;

namespace AppHarbor
{
	public class ApiException : Exception
	{
		public ApiException()
		{
		}

		public ApiException(string message)
			: base(message)
		{
		}
	}
}
