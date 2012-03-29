using System;

namespace AppHarbor
{
	public class AccessTokenConfiguration
	{
		private const string TokenEnvironmentVariable = "AppHarborToken";
		private const EnvironmentVariableTarget TokenEnvironmentVariableTarget = EnvironmentVariableTarget.User;

		public virtual string GetAccessToken()
		{
			return Environment.GetEnvironmentVariable(TokenEnvironmentVariable, TokenEnvironmentVariableTarget);
		}

		public virtual void SetAccessToken(string username, string password)
		{
			throw new NotImplementedException();
			//Environment.SetEnvironmentVariable(TokenEnvironmentVariable, "foo", TokenEnvironmentVariableTarget);
		}
	}
}
