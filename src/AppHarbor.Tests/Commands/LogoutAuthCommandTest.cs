using System.IO;
using AppHarbor.Commands;
using Moq;
using Xunit;

namespace AppHarbor.Tests.Commands
{
	public class LogoutAuthCommandTest
	{
		[Fact]
		public void ShouldLogoutUser()
		{
			var accessTokenConfigurationMock = new Mock<AccessTokenConfiguration>();
			var writer = new Mock<TextWriter>();
			var logoutCommand = new LogoutAuthCommand(accessTokenConfigurationMock.Object, writer.Object);

			logoutCommand.Execute(new string[0]);

			writer.Verify(x => x.WriteLine("Successfully logged out."));
			accessTokenConfigurationMock.Verify(x => x.DeleteAccessToken(), Times.Once());
		}
	}
}
