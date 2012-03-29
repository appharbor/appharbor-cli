using System;

namespace AppHarbor
{
	public class EnvironmentVariableConfiguration
	{
		public virtual string Get(string variable, EnvironmentVariableTarget environmentVariableTarget)
		{
			return Environment.GetEnvironmentVariable(variable, environmentVariableTarget);
		}

		public virtual void Set(string variable, string value, EnvironmentVariableTarget environmentVariableTarget)
		{
			Environment.SetEnvironmentVariable(variable, value, environmentVariableTarget);
		}
	}
}
