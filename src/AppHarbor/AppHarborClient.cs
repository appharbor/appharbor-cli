using System;
using System.Collections.Generic;
using AppHarbor.Model;

namespace AppHarbor
{
	public class AppHarborClient : IAppHarborClient
	{
		private readonly AuthInfo _authInfo;

		public AppHarborClient(string AccessToken)
		{
			_authInfo = new AuthInfo { AccessToken = AccessToken };
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

		private AppHarborApi _api
		{
			get
			{
				try
				{
					return new AppHarborApi(_authInfo);
				}
				catch (ArgumentNullException)
				{
					throw new CommandException("You're not logged in. Log in with \"appharbor login\"");
				}
			}
		}
	}
}
