using System;

namespace AppHarbor
{
	public class GitCommandException : Exception
	{
		public GitCommandException()
		{
		}

		public GitCommandException(string message)
			: base(message)
		{
		}
	}
}
