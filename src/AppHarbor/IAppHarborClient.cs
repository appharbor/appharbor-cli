using System.Collections.Generic;
using AppHarbor.Model;

namespace AppHarbor
{
	public interface IAppHarborClient
	{
		CreateResult<string> CreateApplication(string name, string regionIdentifier = null);
		CreateResult<long> CreateConfigurationVariable(string applicationId, string key, string value);
		Application GetApplication(string id);
		void DeleteApplication(string id);
		IEnumerable<Application> GetApplications();
		User GetUser();
	}
}
