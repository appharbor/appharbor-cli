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
		public void ShouldTryAndSetUpGitRemoteIfPossible([Frozen]Mock<IGitCommand> gitCommand, GitRepositoryConfigurer repositoryConfigurer, User user, string id)
		{
			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);

				repositoryConfigurer.Configure(id, user);

				Assert.Contains("Added \"appharbor\" as a remote repository. Push to AppHarbor with git push appharbor master", writer.ToString());
			}

			gitCommand.Verify(x => x.Execute(string.Format("remote add appharbor {0}", GetRepositoryUrl(id, user))), Times.Once());
		}

		[Theory, AutoCommandData]
		public void ShouldThrowExceptionIfGitRemoteCantBeAdded([Frozen]Mock<IGitCommand> gitCommand, GitRepositoryConfigurer repositoryConfigurer, string id, User user)
		{
			var repositoryUrl = GetRepositoryUrl(id, user);
			gitCommand.Setup(x => x.Execute(string.Format("remote add appharbor {0}", repositoryUrl))).Throws<InvalidOperationException>();

			var exception = Assert.Throws<RepositoryConfigurationException>(() => repositoryConfigurer.Configure(id, user));
			Assert.Equal(string.Format("Couldn't add appharbor repository as a git remote. Repository URL is: {0}", repositoryUrl),
				exception.Message);
		}

		[Theory, AutoCommandData]
		public void ShouldThrowIfGitIsNotInstalled([Frozen]Mock<IGitCommand> gitCommand, GitRepositoryConfigurer repositoryConfigurer, string id, User user)
		{
			gitCommand.Setup(x => x.Execute("--version")).Throws<InvalidOperationException>();

			var exception = Assert.Throws<RepositoryConfigurationException>(() => repositoryConfigurer.Configure(id, user));
			Assert.Equal(string.Format("Git is not installed.", GetRepositoryUrl(id, user)), exception.Message);
		}

		[Theory, AutoCommandData]
		public void ShouldReturnApplicationIdIfAppHarborRemoteExists([Frozen]Mock<IGitCommand> gitCommand, GitRepositoryConfigurer repositoryConfigurer, string id)
		{
			gitCommand.Setup(x => x.Execute("config remote.appharbor.url"))
				.Returns(new string[] { string.Format("https://foo@appharbor.com/{0}.git", id) });

			Assert.Equal(id, repositoryConfigurer.GetApplicationId());
		}

		private static string GetRepositoryUrl(string id, User user)
		{
			return string.Format("https://{0}@appharbor.com/{1}.git", user.Username, id);
		}
	}
}
