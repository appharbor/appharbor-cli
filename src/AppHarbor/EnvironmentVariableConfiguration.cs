using System;

namespace AppHarbor
{
	public class EnvironmentVariableConfiguration
	{
		public void Set(string variable, string value, EnvironmentVariableTarget environmentVariableTarget)
		{
			Environment.SetEnvironmentVariable(variable, value, environmentVariableTarget);
		}
	}
}
