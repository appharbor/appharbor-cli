using System;

namespace AppHarbor
{
	public class ApplicationConfigurationException : CommandException
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
