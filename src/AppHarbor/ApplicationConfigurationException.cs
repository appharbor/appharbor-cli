using System;

namespace AppHarbor
{
	public class ApplicationConfigurationException : Exception
	{
		public ApplicationConfigurationException(string message)
			: base(message)
		{
		}
	}
}
