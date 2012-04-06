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
		public void ShouldReturnApplicationIdIfConfigurationFileExists(Mock<IFileSystem> fileSystem, IGitRepositoryConfigurer repositoryConfigurer, string applicationName)
		{
			var configurationFile = ConfigurationFile;
			var stream = new MemoryStream(Encoding.Default.GetBytes(applicationName));

			fileSystem.Setup(x => x.OpenRead(configurationFile)).Returns(stream);

			var applicationConfiguration = new ApplicationConfiguration(fileSystem.Object, null, repositoryConfigurer);
			Assert.Equal(applicationName, applicationConfiguration.GetApplicationId());
		}

		[Theory, AutoCommandData]
		public void ShouldReturnApplicationIdIfAppHarborRemoteExists([Frozen]Mock<IFileSystem> fileSystem, [Frozen]Mock<IGitExecutor> gitExecutor, ApplicationConfiguration applicationConfiguration, string id)
		{
			fileSystem.Setup(x => x.OpenRead(It.IsAny<string>())).Throws<FileNotFoundException>();

			gitExecutor.Setup(x => x.Execute("config remote.appharbor.url", It.IsAny<DirectoryInfo>()))
				.Returns(new string[] { string.Format("https://foo@appharbor.com/{0}.git", id) });

			var actual = applicationConfiguration.GetApplicationId();

			Assert.Equal(id, actual);
		}

		[Theory, AutoCommandData]
		public void ShouldThrowIfApplicationFileDoesNotExist([Frozen]Mock<IFileSystem> fileSystem, ApplicationConfiguration applicationConfiguration)
		{
			fileSystem.Setup(x => x.OpenRead(It.IsAny<string>())).Throws<FileNotFoundException>();

			var exception = Assert.Throws<ApplicationConfigurationException>(() => applicationConfiguration.GetApplicationId());
			Assert.Equal("Application is not configured", exception.Message);
		}

		[Theory, AutoCommandData]
		public void ShouldOutputRepositoryExceptionIfRepositorySetupFailed([Frozen]Mock<IGitRepositoryConfigurer> repositoryConfigurer, [Frozen]Mock<IFileSystem> fileSystem, ApplicationConfiguration applicationConfiguration, string exceptionMessage)
		{
			fileSystem.Setup(x => x.OpenWrite(ConfigurationFile)).Returns(new MemoryStream());
			repositoryConfigurer.Setup(x => x.Configure(It.IsAny<string>(), It.IsAny<User>()))
				.Throws(new RepositoryConfigurationException(exceptionMessage));

			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);

				applicationConfiguration.SetupApplication(It.IsAny<string>(), It.IsAny<User>());

				var output = writer.ToString();
				Assert.Contains(exceptionMessage, output);
				Assert.Contains(string.Format("Wrote application configuration to {0}", ConfigurationFile), output);
			}
		}

		[Theory, AutoCommandData]
		public void ShouldCreateAppHarborConfigurationFileIfGitSetupFailed([Frozen]Mock<IGitRepositoryConfigurer> repositoryConfigurer, [Frozen]Mock<IFileSystem> fileSystem, ApplicationConfiguration applicationConfiguration, string id, User user)
		{
			repositoryConfigurer.Setup(x => x.Configure(It.IsAny<string>(), It.IsAny<User>())).Throws<RepositoryConfigurationException>();
			Action<MemoryStream> VerifyConfigurationContent = stream => Assert.Equal(Encoding.Default.GetBytes(id), stream.ToArray());

			using (var stream = new DelegateOutputStream(VerifyConfigurationContent))
			{
				fileSystem.Setup(x => x.OpenWrite(ConfigurationFile)).Returns(stream);
				applicationConfiguration.SetupApplication(id, user);
			}

			fileSystem.Verify(x => x.OpenWrite(ConfigurationFile), Times.Once());
		}
	}
}
