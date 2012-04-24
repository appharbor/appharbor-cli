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
		public void ShouldReturnApplicationIdIfConfigurationFileExists(Mock<IFileSystem> fileSystem, TextWriter writer, IGitRepositoryConfigurer repositoryConfigurer, string applicationName)
		{
			var configurationFile = ConfigurationFile;
			var stream = new MemoryStream(Encoding.Default.GetBytes(applicationName));

			fileSystem.Setup(x => x.OpenRead(configurationFile)).Returns(stream);

			var applicationConfiguration = new ApplicationConfiguration(fileSystem.Object, repositoryConfigurer, writer);
			Assert.Equal(applicationName, applicationConfiguration.GetApplicationId());
		}

		[Theory, AutoCommandData]
		public void ShouldThrowIfApplicationFileAndGitConfigurationDoesNotExist([Frozen]Mock<IFileSystem> fileSystem, [Frozen]Mock<IGitRepositoryConfigurer> repositoryConfigurer, ApplicationConfiguration applicationConfiguration)
		{
			fileSystem.Setup(x => x.OpenRead(It.IsAny<string>())).Throws<FileNotFoundException>();
			repositoryConfigurer.Setup(x => x.GetApplicationId()).Throws<RepositoryConfigurationException>();

			var exception = Assert.Throws<ApplicationConfigurationException>(() => applicationConfiguration.GetApplicationId());
			Assert.Equal("Application is not configured in this directory. Configure it by creating (\"create\") an application or by linking (\"link\") an existing application.", exception.Message);
		}

		[Theory, AutoCommandData]
		public void ShouldOutputRepositoryExceptionIfRepositorySetupFailed([Frozen]Mock<IGitRepositoryConfigurer> repositoryConfigurer, [Frozen]Mock<IFileSystem> fileSystem, [Frozen]Mock<TextWriter> writer, ApplicationConfiguration applicationConfiguration, string exceptionMessage)
		{
			fileSystem.Setup(x => x.OpenWrite(ConfigurationFile)).Returns(new MemoryStream());
			repositoryConfigurer.Setup(x => x.Configure(It.IsAny<string>(), It.IsAny<User>()))
				.Throws(new RepositoryConfigurationException(exceptionMessage));

			applicationConfiguration.SetupApplication(It.IsAny<string>(), It.IsAny<User>());

			writer.Verify(x => x.WriteLine(exceptionMessage));
			writer.Verify(x => x.WriteLine("Wrote application configuration to {0}. Make sure not to delete this file", ConfigurationFile));
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

		[Theory, AutoCommandData]
		public void ShouldUnconfigureRepository([Frozen]Mock<IGitRepositoryConfigurer> repositoryConfigurer, ApplicationConfiguration applicationConfiguration)
		{
			applicationConfiguration.RemoveConfiguration();

			repositoryConfigurer.Verify(x => x.Unconfigure());
		}

		[Theory, AutoCommandData]
		public void ShouldRemoveConfigurationFile([Frozen]Mock<IFileSystem> fileSystem, ApplicationConfiguration applicationConfiguration)
		{
			applicationConfiguration.RemoveConfiguration();

			fileSystem.Verify(x => x.Delete(ConfigurationFile));
		}

		[Theory, AutoCommandData]
		public void ShouldStillRemoveConfigurationFileIfGitRemoteIsNotRemoved([Frozen]Mock<IGitRepositoryConfigurer> repositoryConfigurer, [Frozen]Mock<IFileSystem> fileSystem, ApplicationConfiguration applicationConfiguration)
		{
			repositoryConfigurer.Setup(x => x.Unconfigure()).Throws<RepositoryConfigurationException>();

			applicationConfiguration.RemoveConfiguration();

			fileSystem.Verify(x => x.Delete(It.IsAny<string>()));
		}
	}
}
