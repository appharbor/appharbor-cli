using System.IO;
using System.Text;
using Moq;
using Xunit;

namespace AppHarbor.Tests
{
	public class ApplicationConfigurationTest
	{
		[Fact]
		public void ShouldReturnApplicationIdIfConfigurationFileExists()
		{
			var fileSystem = new Mock<IFileSystem>();
			var applicationName = "bar";

			var configurationFile = Path.GetFullPath(".appharbor");
			var stream = new MemoryStream(Encoding.Default.GetBytes(applicationName));

			fileSystem.Setup(x => x.OpenRead(configurationFile)).Returns(stream);

			var applicationConfiguration = new ApplicationConfiguration(fileSystem.Object);
			Assert.Equal(applicationName, applicationConfiguration.GetApplicationId());
		}
	}
}
