using AppHarbor.Model;

namespace AppHarbor
{
	public interface IGitRepositoryConfigurer
	{
		void Configure(string id, User user);
		string GetApplicationId();
	}
}
