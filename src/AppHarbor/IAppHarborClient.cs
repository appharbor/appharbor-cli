using System.Collections.Generic;
using AppHarbor.Model;

namespace AppHarbor
{
	public interface IAppHarborClient
	{
		CreateResult CreateApplication(string name, string regionIdentifier = null);
		void CreateConfigurationVariable(string applicationId, string key, string value);
		void CreateHostname(string applicationId, string hostname);
		Application GetApplication(string id);
		IEnumerable<Build> GetBuilds(string applicationId);
		IEnumerable<ConfigurationVariable> GetConfigurationVariables(string applicationId);
		IEnumerable<Hostname> GetHostnames(string applicationId);
		void DeleteApplication(string id);
		IEnumerable<Application> GetApplications();
		User GetUser();
		void RemoveConfigurationVariable(string applicationId, string key);
		void RemoveHostname(string applicationId, string hostname);
	}
}
