using Xunit;
using Moq;
using AppHarbor.Commands;

namespace AppHarbor.Tests.Commands
{
	public class LogoutCommandTest
	{
		[Fact]
		public void ShouldLogoutUser()
		{
			var accessTokenConfigurationMock = new Mock<AccessTokenConfiguration>();
			var logoutCommand = new LogoutCommand(accessTokenConfigurationMock.Object);

			logoutCommand.Execute(new string[0]);
			accessTokenConfigurationMock.Verify(x => x.DeleteAccessToken(), Times.Once());
		}
	}
}
