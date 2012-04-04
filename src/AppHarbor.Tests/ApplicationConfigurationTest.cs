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
	public class ApplicationConfigurationTest
	{
		public static string ConfigurationFile = Path.GetFullPath(".appharbor");

		[Fact]
		public void ShouldReturnApplicationIdIfConfigurationFileExists()
		{
			var fileSystem = new Mock<IFileSystem>();
			var applicationName = "bar";

			var configurationFile = ConfigurationFile;
			var stream = new MemoryStream(Encoding.Default.GetBytes(applicationName));

			fileSystem.Setup(x => x.OpenRead(configurationFile)).Returns(stream);

			var applicationConfiguration = new ApplicationConfiguration(fileSystem.Object, null);
			Assert.Equal(applicationName, applicationConfiguration.GetApplicationId());
		}

		[Fact]
		public void ShouldThrowIfApplicationFileDoesNotExist()
		{
			var fileSystem = new InMemoryFileSystem();
			var applicationConfiguration = new ApplicationConfiguration(fileSystem, null);

			var exception = Assert.Throws<ApplicationConfigurationException>(() => applicationConfiguration.GetApplicationId());
			Assert.Equal("Application is not configured", exception.Message);
		}

		[Theory, AutoCommandData]
		public void ShouldTryAndSetUpGitRemoteIfPossible([Frozen]Mock<IGitExecutor> gitExecutor, [Frozen]Mock<IAppHarborClient> client, ApplicationConfiguration applicationConfiguration, User user, string id)
		{
			gitExecutor.Setup(x => x.IsInstalled()).Returns(true);
			client.Setup(x => x.GetUser()).Returns(user);

			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);

				applicationConfiguration.SetupApplication(id, client.Object);

				Assert.Contains("Added \"appharbor\" as a remote repository. Push to AppHarbor with git push appharbor master", writer.ToString());
			}

			var gitCommand = string.Format("remote add appharbor https://{0}@appharbor.com/{1}.git", user.Username, id);
			gitExecutor.Verify(x =>
				x.Execute(gitCommand, It.Is<DirectoryInfo>(y => y.FullName == Directory.GetCurrentDirectory())),
				Times.Once());
		}

		[Theory, AutoCommandData]
		public void ShouldShowRepositoryUrlIfGitSetupFailed([Frozen]Mock<IGitExecutor> gitExecutor, ApplicationConfiguration applicationConfiguration, IAppHarborClient client, string id)
		{
			gitExecutor.Setup(x => x.IsInstalled()).Returns(true);
			gitExecutor.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<DirectoryInfo>())).Throws<InvalidOperationException>();

			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);

				applicationConfiguration.SetupApplication(id, client);

				Assert.Contains(string.Format("Couldn't add appharbor repository as a git remote. Repository URL is: https://@appharbor.com/{0}.git", id), writer.ToString());
			}
		}
	}
}
