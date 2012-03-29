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
		public void ShouldAskForUsernameAndPassword()
		{
			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);

				using (var reader = new StringReader(string.Format("foo{0}bar{0}", Environment.NewLine)))
				{
					Console.SetIn(reader);

					var loginCommand = new LoginCommand(null, null);
					loginCommand.Execute(new string[] { });

					var expected = string.Format("Username:{0}Password:{0}", Environment.NewLine);
					Assert.Equal(expected, writer.ToString());
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
