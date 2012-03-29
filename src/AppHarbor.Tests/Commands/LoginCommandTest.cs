using System;
using System.IO;
using AppHarbor.Commands;
using Moq;
using Xunit;

namespace AppHarbor.Tests.Commands
{
	public class LoginCommandTest
	{
		[Fact]
		public void ShouldSetAppHarborTokenIfUserIsntLoggedIn()
		{
			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);

				var username = "foo";
				var password = "bar";

				using (var reader = new StringReader(string.Format("{0}{2}{1}{2}", username, password, Environment.NewLine)))
				{
					Console.SetIn(reader);

					var accessTokenConfigurationMock = new Mock<AccessTokenConfiguration>();
					accessTokenConfigurationMock.Setup(x => x.Get()).Returns((string)null);

					var loginCommand = new LoginCommand(accessTokenConfigurationMock.Object);
					loginCommand.Execute(new string[] { });

					var expected = string.Format("Username:{0}Password:{0}", Environment.NewLine);
					Assert.Equal(expected, writer.ToString());
					accessTokenConfigurationMock.Verify(x => x.Set(username, password), Times.Once());
				}
			}
		}

		[Fact]
		public void ShouldThrowIfUserIsAlreadyLoggedIn()
		{
			var accessTokenConfigurationMock = new Mock<AccessTokenConfiguration>();
			accessTokenConfigurationMock.Setup(x => x.Get()).Returns("foo");

			var loginCommand = new LoginCommand(accessTokenConfigurationMock.Object);

			var exception = Assert.Throws<CommandException>(() => loginCommand.Execute(new string[] { }));
			Assert.Equal("You're already logged in", exception.Message);
		}
	}
}
