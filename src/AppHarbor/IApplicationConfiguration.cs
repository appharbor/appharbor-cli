using System;

namespace AppHarbor
{
	public interface IApplicationConfiguration
	{
		void RemoveConfiguration();
		string GetApplicationId();
		void SetupApplication(string id, AppHarbor.Model.User user);
	}
}
