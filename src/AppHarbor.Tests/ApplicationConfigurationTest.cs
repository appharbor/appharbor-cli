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

		[Theory, AutoCommandData]
		public void ShouldReturnApplicationIdIfConfigurationFileExists(Mock<IFileSystem> fileSystem, string applicationName)
		{
			var configurationFile = ConfigurationFile;
			var stream = new MemoryStream(Encoding.Default.GetBytes(applicationName));

			fileSystem.Setup(x => x.OpenRead(configurationFile)).Returns(stream);

			var applicationConfiguration = new ApplicationConfiguration(fileSystem.Object, null);
			Assert.Equal(applicationName, applicationConfiguration.GetApplicationId());
		}

		[Theory, AutoCommandData]
		public void ShouldThrowIfApplicationFileDoesNotExist(InMemoryFileSystem fileSystem, IGitExecutor gitExecutor)
		{
			var applicationConfiguration = new ApplicationConfiguration(fileSystem, gitExecutor);

			var exception = Assert.Throws<ApplicationConfigurationException>(() => applicationConfiguration.GetApplicationId());
			Assert.Equal("Application is not configured", exception.Message);
		}

		[Theory, AutoCommandData]
		public void ShouldTryAndSetUpGitRemoteIfPossible([Frozen]Mock<IGitExecutor> gitExecutor, [Frozen]Mock<IAppHarborClient> client, ApplicationConfiguration applicationConfiguration, User user, string id)
		{
			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);

				applicationConfiguration.SetupApplication(id, user);

				Assert.Contains("Added \"appharbor\" as a remote repository. Push to AppHarbor with git push appharbor master", writer.ToString());
			}

			var gitCommand = string.Format("remote add appharbor https://{0}@appharbor.com/{1}.git", user.Username, id);
			gitExecutor.Verify(x =>
				x.Execute(gitCommand, It.Is<DirectoryInfo>(y => y.FullName == Directory.GetCurrentDirectory())),
				Times.Once());
		}

		[Theory, AutoCommandData]
		public void ShouldShowRepositoryUrlIfGitSetupFailed([Frozen]Mock<IGitExecutor> gitExecutor, ApplicationConfiguration applicationConfiguration, User user, string id)
		{
			gitExecutor.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<DirectoryInfo>())).Throws<InvalidOperationException>();

			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);

				applicationConfiguration.SetupApplication(id, user);

				Assert.Contains(string.Format("Couldn't add appharbor repository as a git remote. Repository URL is: https://{0}@appharbor.com/{1}.git", user.Username, id), writer.ToString());
			}
		}
	}
}
