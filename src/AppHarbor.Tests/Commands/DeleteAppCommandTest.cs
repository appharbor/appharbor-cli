using AppHarbor.Commands;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests.Commands
{
	public class DeleteAppCommandTest
	{
		[Theory, AutoCommandData]
		public void ShouldDeleteFromApi([Frozen]Mock<IAppHarborClient> appharborClient,
			[Frozen]Mock<IApplicationConfiguration> applicationConfiguration,
			DeleteAppCommand command,
			string id)
		{
			applicationConfiguration.Setup(x => x.GetApplicationId()).Returns(id);
			appharborClient.Setup(x => x.DeleteApplication(id));

			command.Run(new string[0]);

			appharborClient.VerifyAll();
			applicationConfiguration.VerifyAll();
			applicationConfiguration.Verify(x => x.RemoveConfiguration());
		}
	}
}
