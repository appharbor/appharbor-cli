using System;
using System.IO;
using System.Text;
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
		public void ShouldTryAndSetUpGitRemoteIfPossible([Frozen]Mock<IGitCommand> gitCommand, [Frozen]Mock<TextWriter> writer, GitRepositoryConfigurer repositoryConfigurer, User user, string id)
		{
			repositoryConfigurer.Configure(id, user);

			writer.Verify(x => x.WriteLine("Added \"appharbor\" as a remote repository. Push to AppHarbor with git push appharbor master."));
			gitCommand.Verify(x => x.Execute(string.Format("remote add appharbor {0}", GetRepositoryUrl(id, user))), Times.Once());
		}

		[Theory, AutoCommandData]
		public void ShouldThrowExceptionIfGitRemoteCantBeAdded([Frozen]Mock<IGitCommand> gitCommand, GitRepositoryConfigurer repositoryConfigurer, string id, User user)
		{
			var repositoryUrl = GetRepositoryUrl(id, user);
			gitCommand.Setup(x => x.Execute(string.Format("remote add appharbor {0}", repositoryUrl))).Throws<GitCommandException>();

			var exception = Assert.Throws<RepositoryConfigurationException>(() => repositoryConfigurer.Configure(id, user));
			Assert.Equal(string.Format("Couldn't add appharbor repository as a git remote. Repository URL is: {0}.", repositoryUrl),
				exception.Message);
		}

		[Theory, AutoCommandData]
		public void ShouldThrowIfGitIsNotInstalled([Frozen]Mock<IGitCommand> gitCommand, GitRepositoryConfigurer repositoryConfigurer, string id, User user)
		{
			gitCommand.Setup(x => x.Execute("--version")).Throws<GitCommandException>();

			var exception = Assert.Throws<RepositoryConfigurationException>(() => repositoryConfigurer.Configure(id, user));
			Assert.Equal(string.Format("Git is not installed.", GetRepositoryUrl(id, user)), exception.Message);
		}

		[Theory, AutoCommandData]
		public void ShouldAskForGitInitializationAndThrowIfNotWanted([Frozen]Mock<IGitCommand> gitCommand, [Frozen]Mock<TextReader> reader, [Frozen]Mock<TextWriter> writer, GitRepositoryConfigurer repositoryConfigurer, string id, User user)
		{
			gitCommand.Setup(x => x.Execute("status")).Throws<GitCommandException>();
			reader.Setup(x => x.ReadLine()).Returns("n");

			var exception = Assert.Throws<RepositoryConfigurationException>(() => repositoryConfigurer.Configure(id, user));

			Assert.Equal("Git repository was not initialized.", exception.Message);
			writer.Verify(x => x.Write("Git repository is not initialized in this folder. Do you want to initialize it (type \"y\")?"));
			reader.VerifyAll();
		}

		[Theory, AutoCommandData]
		public void ShouldInitializeRepositoryIfUserWantIt([Frozen]Mock<IFileSystem> fileSystem, [Frozen]Mock<TextReader> reader, [Frozen]Mock<TextWriter> writer, [Frozen]Mock<IGitCommand> gitCommand, GitRepositoryConfigurer repositoryConfigurer, string id, User user)
		{
			gitCommand.Setup(x => x.Execute("status")).Throws<GitCommandException>();
			reader.Setup(x => x.ReadLine()).Returns("y");
			var gitIgnoreFile = Path.Combine(Directory.GetCurrentDirectory(), ".gitignore");
			Action<MemoryStream> VerifyGitIgnoreContent = stream =>
				Assert.Equal(Encoding.Default.GetBytes(GitRepositoryConfigurer.DefaultGitIgnore), stream.ToArray());

			using (var stream = new DelegateOutputStream(VerifyGitIgnoreContent))
			{
				fileSystem.Setup(x => x.OpenWrite(gitIgnoreFile)).Returns(stream);
				repositoryConfigurer.Configure(id, user);
			}

			fileSystem.Verify(x => x.OpenWrite(gitIgnoreFile), Times.Once());
			gitCommand.Verify(x => x.Execute("init"), Times.Once());
			writer.Verify(x => x.WriteLine("Git repository was initialized with default .gitignore file."));
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

		[Theory, AutoCommandData]
		public void ShouldThrowIfRemovingRemoteIsUnsuccessful([Frozen]Mock<IGitCommand> gitCommand, GitRepositoryConfigurer repositoryConfigurer)
		{
			gitCommand.Setup(x => x.Execute("remote rm appharbor")).Throws<GitCommandException>();

			Assert.Throws<RepositoryConfigurationException>(() => repositoryConfigurer.Unconfigure());

			gitCommand.VerifyAll();
		}
	}
}
