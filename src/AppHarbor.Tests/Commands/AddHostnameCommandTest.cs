using AppHarbor.Commands;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests.Commands
{
	public class AddHostnameCommandTest
	{
		[Theory, AutoCommandData]
		public void ShouldThrowIfNoArguments(AddHostnameCommand command)
		{
			Assert.Throws<CommandException>(() => command.Execute(new string[0]));
		}

		[Theory]
		[InlineAutoCommandData("example.com")]
		public void ShouldAddConfigurationVariables(string hostname,
			[Frozen]Mock<IApplicationConfiguration> applicationConfiguration,
			[Frozen]Mock<IAppHarborClient> client,
			AddHostnameCommand command, string applicationId)
		{
			applicationConfiguration.Setup(x => x.GetApplicationId()).Returns(applicationId);

			command.Execute(new string[] { hostname });

			client.Verify(x => x.CreateHostname(applicationId, hostname));
		}
	}
}
