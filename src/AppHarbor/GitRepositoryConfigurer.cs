using System;
using AppHarbor.Model;

namespace AppHarbor
{
	public class GitRepositoryConfigurer
	{
		private readonly IGitExecutor _executor;

		public GitRepositoryConfigurer(IGitExecutor executor)
		{
			_executor = executor;
		}

		public void Configure(string id, User user)
		{
			throw new NotImplementedException();
		}
	}
}
