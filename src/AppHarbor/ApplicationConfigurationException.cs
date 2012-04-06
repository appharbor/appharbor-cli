using System;

namespace AppHarbor
{
	public class ApplicationConfigurationException : Exception
	{
		public ApplicationConfigurationException()
		{
		}

		public ApplicationConfigurationException(string message)
			: base(message)
		{
		}
	}
}
