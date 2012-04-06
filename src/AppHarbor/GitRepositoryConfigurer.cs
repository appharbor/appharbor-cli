using System;
using System.IO;
using AppHarbor.Model;

namespace AppHarbor
{
	public class GitRepositoryConfigurer : IGitRepositoryConfigurer
	{
		private readonly IGitExecutor _executor;

		public GitRepositoryConfigurer(IGitExecutor executor)
		{
			_executor = executor;
		}

		public void Configure(string id, User user)
		{
			var repositoryUrl = string.Format("https://{0}@appharbor.com/{1}.git", user.Username, id);

			try
			{
				_executor.Execute(string.Format("remote add appharbor {0}", repositoryUrl),
					new DirectoryInfo(Directory.GetCurrentDirectory()));

				Console.WriteLine("Added \"appharbor\" as a remote repository. Push to AppHarbor with git push appharbor master");
			}
			catch (InvalidOperationException)
			{
				throw new RepositoryConfigurationException(
					string.Format("Couldn't add appharbor repository as a git remote. Repository URL is: {0}", repositoryUrl));
			}
		}
	}
}
