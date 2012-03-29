namespace AppHarbor
{
	public class AppHarborClient
	{
		private readonly AuthInfo _authInfo;

		public AppHarborClient(string AccessToken)
		{
			_authInfo = new AuthInfo { AccessToken = AccessToken };
		}

		public void CreateApplication(string name, string regionIdentifier)
		{
			var appHarborApi = GetAppHarborApi();
			appHarborApi.CreateApplication(name, regionIdentifier);
		}

		private AppHarborApi GetAppHarborApi()
		{
			return new AppHarborApi(_authInfo);
		}
	}
}
