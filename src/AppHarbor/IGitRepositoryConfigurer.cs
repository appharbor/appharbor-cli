using AppHarbor.Model;

namespace AppHarbor
{
	interface IGitRepositoryConfigurer
	{
		void Configure(string id, User user);
	}
}
