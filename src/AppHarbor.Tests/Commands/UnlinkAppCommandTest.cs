using AppHarbor.Commands;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests.Commands
{
	public class UnlinkAppCommandTest
	{
		[Theory, AutoCommandData]
		public void ShouldUnlinkApplication([Frozen]Mock<IApplicationConfiguration> applicationConfiguration, UnlinkAppCommand command)
		{
			command.Execute(new string[0]);
			applicationConfiguration.Verify(x => x.DeleteApplication());
		}
	}
}
