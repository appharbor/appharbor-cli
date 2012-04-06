using System;
using System.Collections.Generic;
using AppHarbor.Model;

namespace AppHarbor
{
	public class AppHarborClient : IAppHarborClient
	{
		private readonly AppHarborApi _api;

		public AppHarborClient(string AccessToken)
		{
			var authInfo = new AuthInfo { AccessToken = AccessToken };
			try
			{
				_api =  new AppHarborApi(authInfo);
			}
			catch (ArgumentNullException)
			{
				throw new CommandException("You're not logged in. Log in with \"appharbor login\"");
			}
		}

		public CreateResult<string> CreateApplication(string name, string regionIdentifier = null)
		{
			return _api.CreateApplication(name, regionIdentifier);
		}

		public Application GetApplication(string id)
		{
			return _api.GetApplication(id);
		}

		public IEnumerable<Application> GetApplications()
		{
			return _api.GetApplications();
		}

		public User GetUser()
		{
			return _api.GetUser();
		}
	}
}
