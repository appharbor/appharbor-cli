using System;
using System.IO;
using AppHarbor.Model;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests
{
	public class GitRepositoryConfigurerTest
	{
		[Theory, AutoCommandData]
		public void ShouldTryAndSetUpGitRemoteIfPossible([Frozen]Mock<IGitExecutor> gitExecutor, [Frozen]Mock<IAppHarborClient> client, GitRepositoryConfigurer repositoryConfigurer, User user, string id)
		{
			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);

				repositoryConfigurer.Configure(id, user);

				Assert.Contains("Added \"appharbor\" as a remote repository. Push to AppHarbor with git push appharbor master", writer.ToString());
			}

			var gitCommand = string.Format("remote add appharbor https://{0}@appharbor.com/{1}.git", user.Username, id);
			gitExecutor.Verify(x =>
				x.Execute(gitCommand, It.Is<DirectoryInfo>(y => y.FullName == Directory.GetCurrentDirectory())),
				Times.Once());
		}
	}
}
