using System;

namespace AppHarbor
{
	public class AccessTokenConfiguration : IAccessTokenConfiguration
	{
		private const string TokenEnvironmentVariable = "AppHarborToken";
		private const EnvironmentVariableTarget TokenEnvironmentVariableTarget = EnvironmentVariableTarget.User;

		public virtual void DeleteAccessToken()
		{
			Environment.SetEnvironmentVariable(TokenEnvironmentVariable, null, TokenEnvironmentVariableTarget);
		}

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
