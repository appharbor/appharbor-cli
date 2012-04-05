using Xunit;
using Moq;
using AppHarbor.Commands;
using System.IO;
using System;

namespace AppHarbor.Tests.Commands
{
	public class LogoutAuthCommandTest
	{
		[Fact]
		public void ShouldLogoutUser()
		{
			var accessTokenConfigurationMock = new Mock<AccessTokenConfiguration>();
			var logoutCommand = new LogoutAuthCommand(accessTokenConfigurationMock.Object);

			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);

				logoutCommand.Execute(new string[0]);

				Assert.Contains("Successfully logged out.", writer.ToString());
			}
			accessTokenConfigurationMock.Verify(x => x.DeleteAccessToken(), Times.Once());
		}
	}
}
