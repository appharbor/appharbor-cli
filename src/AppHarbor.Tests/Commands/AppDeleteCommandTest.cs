using AppHarbor.Commands;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests.Commands
{
	public class AppDeleteCommandTest
	{
		[Theory, AutoCommandData]
		public void ShouldDeleteFromApi([Frozen]Mock<IAppHarborClient> appharborClient,
			[Frozen]Mock<IApplicationConfiguration> applicationConfiguration,
			AppDeleteCommand command,
			string id)
		{
			applicationConfiguration.Setup(x => x.GetApplicationId()).Returns(id);
			appharborClient.Setup(x => x.DeleteApplication(id));

			command.Execute(new string[0]);

			appharborClient.VerifyAll();
			applicationConfiguration.VerifyAll();
		}
	}
}
