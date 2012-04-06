using System;
using System.IO;
using AppHarbor.Commands;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests.Commands
{
	public class LoginAuthCommandTest
	{
		[Theory, AutoData]
		public void ShouldSetAppHarborTokenIfUserIsntLoggedIn(string username, string password, [Frozen]Mock<IAccessTokenConfiguration> accessTokenConfigurationMock)
		{
			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);

				using (var reader = new StringReader(string.Format("{0}{2}{1}{2}", username, password, Environment.NewLine)))
				{
					Console.SetIn(reader);

					accessTokenConfigurationMock.Setup(x => x.GetAccessToken()).Returns((string)null);
					var loginCommand = new Mock<LoginAuthCommand>(accessTokenConfigurationMock.Object);
					loginCommand.Setup(x => x.GetAccessToken(username, password)).Returns("foo");
					loginCommand.Object.Execute(new string[] { });

					var expected = string.Format("Username: Password: Successfully logged in as {1}{0}", Environment.NewLine, username);
					Assert.Equal(expected, writer.ToString());
					accessTokenConfigurationMock.Verify(x => x.SetAccessToken("foo"), Times.Once());
				}
			}
		}

		[Theory, AutoData]
		public void ShouldThrowIfUserIsAlreadyLoggedIn([Frozen]Mock<AccessTokenConfiguration> accessTokenConfigurationMock)
		{
			accessTokenConfigurationMock.Setup(x => x.GetAccessToken()).Returns("foo");

			var loginCommand = new LoginAuthCommand(accessTokenConfigurationMock.Object);

			var exception = Assert.Throws<CommandException>(() => loginCommand.Execute(new string[] { }));
			Assert.Equal("You're already logged in", exception.Message);
		}
	}
}
