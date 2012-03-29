namespace AppHarbor
{
	public class AppHarborClient
	{
		private readonly AuthInfo _authInfo;

		public AppHarborClient(AuthInfo authInfo)
		{
			_authInfo = authInfo;
		}

		public void CreateApplication(string name, string regionIdentifier)
		{
			var appHarborApi = new AppHarborApi(_authInfo);
			appHarborApi.CreateApplication(name, regionIdentifier);
		}
	}
}
