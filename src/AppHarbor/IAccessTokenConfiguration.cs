using System;

namespace AppHarbor
{
	public interface IAccessTokenConfiguration
	{
		void DeleteAccessToken();
		string GetAccessToken();
		void SetAccessToken(string username, string password);
	}
}
