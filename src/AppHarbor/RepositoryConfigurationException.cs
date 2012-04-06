using System;

namespace AppHarbor
{
	public class RepositoryConfigurationException : Exception
	{
		public RepositoryConfigurationException()
		{
		}

		public RepositoryConfigurationException(string message)
			: base(message)
		{
		}
	}
}
