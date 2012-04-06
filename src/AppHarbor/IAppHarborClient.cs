using System.Collections.Generic;
using AppHarbor.Model;

namespace AppHarbor
{
	public interface IAppHarborClient
	{
		CreateResult<string> CreateApplication(string name, string regionIdentifier = null);
		Application GetApplication(string id);
		IEnumerable<Application> GetApplications();
		User GetUser();
	}
}
