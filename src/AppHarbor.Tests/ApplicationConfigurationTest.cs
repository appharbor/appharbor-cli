using System.IO;
using System.Text;
using Moq;
using Xunit;

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

			var applicationConfiguration = new ApplicationConfiguration(fileSystem.Object);
			Assert.Equal(applicationName, applicationConfiguration.GetApplicationId());
		}

		[Fact]
		public void ShouldThrowIfApplicationFileDoesNotExist()
		{
			var fileSystem = new InMemoryFileSystem();
			var applicationConfiguration = new ApplicationConfiguration(fileSystem);

			Assert.Throws<ApplicationConfigurationException>(() => applicationConfiguration.GetApplicationId());
		}
	}
}
