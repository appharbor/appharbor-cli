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

					var accessTokenFetcher = new Mock<AccessTokenFetcher>();
					var accessToken = "baz";
					accessTokenFetcher.Setup(x => x.Get(username, password)).Returns(accessToken);

					var environmentVariableConfiguration = new Mock<EnvironmentVariableConfiguration>();
					var tokenConfigurationVariable = "AppHarborToken";
					environmentVariableConfiguration.Setup(x => x.Get(tokenConfigurationVariable, EnvironmentVariableTarget.User)).Returns((string)null);

					var loginCommand = new LoginCommand(accessTokenFetcher.Object, environmentVariableConfiguration.Object);
					loginCommand.Execute(new string[] { });

					var expected = string.Format("Username:{0}Password:{0}", Environment.NewLine);
					Assert.Equal(expected, writer.ToString());
					environmentVariableConfiguration.Verify(x => x.Set("AppHarborToken", accessToken, EnvironmentVariableTarget.User), Times.Once());
				}
			}
		}

		[Fact]
		public void ShouldThrowIfUserIsAlreadyLoggedIn()
		{
			var environmentVariableConfigurationMock = new Mock<EnvironmentVariableConfiguration>();
			environmentVariableConfigurationMock.Setup(x => 
				x.Get(It.IsAny<string>(), It.IsAny<EnvironmentVariableTarget>())).Returns("bar");

			var loginCommand = new LoginCommand(null, environmentVariableConfigurationMock.Object);

			var exception = Assert.Throws<CommandException>(() => loginCommand.Execute(new string[] { }));
			Assert.Equal("You're already logged in", exception.Message);
		}
	}
}
