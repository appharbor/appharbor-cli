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
			var result = _api.CreateApplication(name, regionIdentifier);
			if (result.Status != CreateStatus.Created)
			{
				throw new ApiException();
			}

			return result;
		}

		public void DeleteApplication(string id)
		{
			if (!_api.DeleteApplication(id))
			{
				throw new ApiException();
			}
		}

		public Application GetApplication(string id)
		{
			var application = _api.GetApplication(id);
			if (string.IsNullOrEmpty(application.Slug))
			{
				throw new ApiException();
			}

			return application;
		}

		public IEnumerable<Application> GetApplications()
		{
			var applications = _api.GetApplications();
			if (applications == null)
			{
				throw new ApiException();
			}

			return applications;
		}

		public User GetUser()
		{
			var user = _api.GetUser();
			if (user == null)
			{
				throw new ApiException();
			}
			return user;
		}


		public CreateResult<long> CreateConfigurationVariable(string key, string value)
		{
			throw new NotImplementedException();
		}
	}
}
