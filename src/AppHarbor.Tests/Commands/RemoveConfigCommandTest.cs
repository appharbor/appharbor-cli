using AppHarbor.Commands;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests.Commands
{
	public class RemoveConfigCommandTest
	{
		[Theory, AutoCommandData]
		public void ShouldThrowIfNoArguments(RemoveConfigCommand command)
		{
			Assert.Throws<CommandException>(() => command.Execute(new string[0]));
		}

		[Theory]
		[InlineAutoCommandData("foo")]
		[InlineAutoCommandData("foo bar baz")]
		public void ShouldAddConfigurationVariables(string arguments,
			[Frozen]Mock<IApplicationConfiguration> applicationConfiguration,
			[Frozen]Mock<IAppHarborClient> client,
			RemoveConfigCommand command, string applicationId)
		{
			applicationConfiguration.Setup(x => x.GetApplicationId()).Returns(applicationId);
			var keysToDelete = arguments.Split();

			command.Execute(keysToDelete);

			foreach (var key in keysToDelete)
			{
				client.Verify(x => x.RemoveConfigurationVariable(applicationId, key));
			}
		}
	}
}
