using System;

namespace AppHarbor
{
	public interface IApplicationConfiguration
	{
		string GetApplicationId();
		void SetupApplication(string id, AppHarbor.Model.User user);
	}
}
