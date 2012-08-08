using AppHarbor.Commands;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests.Commands
{
	public class RemoveHostnameCommandTest
	{
		[Theory, AutoCommandData]
		public void ShouldThrowIfNoArguments(RemoveHostnameCommand command)
		{
			Assert.Throws<CommandException>(() => command.Run(new string[0]));
		}

		[Theory]
		[InlineAutoCommandData("example.com")]
		public void ShouldRemoveHostname(string hostname,
			[Frozen]Mock<IApplicationConfiguration> applicationConfiguration,
			[Frozen]Mock<IAppHarborClient> client,
			RemoveHostnameCommand command, string applicationId)
		{
			applicationConfiguration.Setup(x => x.GetApplicationId()).Returns(applicationId);

			command.Run(new string[] { hostname });

			client.Verify(x => x.RemoveHostname(applicationId, hostname));
		}
	}
}
