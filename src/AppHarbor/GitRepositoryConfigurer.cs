namespace AppHarbor
{
	public class GitRepositoryConfigurer
	{
		private readonly IGitExecutor _executor;

		public GitRepositoryConfigurer(IGitExecutor executor)
		{
			_executor = executor;
		}
	}
}
