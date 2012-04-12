using System;
using System.Collections.Generic;
using System.Linq;
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

		public void CreateConfigurationVariable(string applicationId, string key, string value)
		{
			var result = _api.CreateConfigurationVariable(applicationId, key, value);

			switch (result.Status)
			{
				case CreateStatus.Created:
					break;
				case CreateStatus.AlreadyExists:
					throw new CommandException(string.Format("The configuration variable key \"{0}\" already exists"));
				default:
					throw new ApiException();
			}
		}

		public void RemoveConfigurationVariable(string applicationId, string key)
		{
			ConfigurationVariable configurationVariable;
			try
			{
				configurationVariable = _api.GetConfigurationVariables(applicationId)
					.Single(x => x.Key.ToLower() == key.ToLower());
			}
			catch (InvalidOperationException)
			{
				throw new CommandException(string.Format("The configuration variable key \"{0}\" could not be found.", key));
			}

			if (!_api.DeleteConfigurationVariable(applicationId, configurationVariable.ID))
			{
				throw new ApiException();
			}
		}

		public IList<ConfigurationVariable> GetConfigurationVariables(string applicationId)
		{
			var configurationVariables = _api.GetConfigurationVariables(applicationId);
			if (configurationVariables == null)
			{
				throw new ApiException();
			}

			return configurationVariables;
		}

		public IList<Hostname> GetHostnames(string applicationId)
		{
			var hostnames = _api.GetHostnames(applicationId);
			if (hostnames == null)
			{
				throw new ApiException();
			}

			return hostnames;
		}

		public void CreateHostname(string applicationId, string hostname)
		{
			var result = _api.CreateHostname(applicationId, hostname);
			if (result.Status != CreateStatus.Created)
			{
				throw new ApiException();
			}
		}


		public void RemoveHostname(string applicationId, string value)
		{
			Hostname hostname;
			try
			{
				hostname = _api.GetHostnames(applicationId)
					.Single(x => x.Value.ToLower() == value.ToLower());
			}
			catch (InvalidOperationException)
			{
				throw new CommandException(string.Format("The hostname \"{0}\" could not be found.", value));
			}

			_api.DeleteHostname(applicationId, hostname.ID);
		}
	}
}
