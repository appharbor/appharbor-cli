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
		public void ShouldTryAndSetUpGitRemoteIfPossible([Frozen]Mock<IGitExecutor> gitExecutor, GitRepositoryConfigurer repositoryConfigurer, User user, string id)
		{
			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);

				repositoryConfigurer.Configure(id, user);

				Assert.Contains("Added \"appharbor\" as a remote repository. Push to AppHarbor with git push appharbor master", writer.ToString());
			}

			var gitCommand = string.Format("remote add appharbor {0}", GetRepositoryUrl(id, user));
			gitExecutor.Verify(x =>
				x.Execute(gitCommand, It.Is<DirectoryInfo>(y => y.FullName == Directory.GetCurrentDirectory())),
				Times.Once());
		}

		[Theory, AutoCommandData]
		public void ShouldThrowExceptionIfGitCommandCantBeExecuted([Frozen]Mock<IGitExecutor> executor, GitRepositoryConfigurer repositoryConfigurer, string id, User user)
		{
			executor.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<DirectoryInfo>())).Throws<InvalidOperationException>();

			var exception = Assert.Throws<RepositoryConfigurationException>(() => repositoryConfigurer.Configure(id, user));
			Assert.Equal(string.Format("Couldn't add appharbor repository as a git remote. Repository URL is: {0}", GetRepositoryUrl(id, user)),
				exception.Message);
		}

		private static string GetRepositoryUrl(string id, User user)
		{
			return string.Format("https://{0}@appharbor.com/{1}.git", user.Username, id);
		}
	}
}
