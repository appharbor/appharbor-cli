using AppHarbor.Model;

namespace AppHarbor
{
	public interface IApplicationConfiguration
	{
		void RemoveConfiguration();
		string GetApplicationId();
		void SetupApplication(string id, User user);
	}
}
