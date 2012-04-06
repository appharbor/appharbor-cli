using System;

namespace AppHarbor
{
	public class GitCommandException : Exception
	{
		public GitCommandException(string message)
			: base(message)
		{
		}
	}
}
