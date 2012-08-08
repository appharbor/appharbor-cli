using System.Collections.Generic;
using AppHarbor.Commands;
using AppHarbor.Model;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests.Commands
{
	public class LinkAppCommandTest
	{
		[Theory, AutoCommandData]
		public void ShouldThrowIfNoArguments(LinkAppCommand command)
		{
			Assert.Throws<CommandException>(() => command.Run(new string[0]));
		}

		[Theory, AutoCommandData]
		public void ShouldSetupApplication([Frozen]Mock<IApplicationConfiguration> applicationConfiguration,
			[Frozen]Mock<IAppHarborClient> appharborClient,
			LinkAppCommand command,
			Application application,
			User user)
		{
			appharborClient.Setup(x => x.GetApplication(application.Slug)).Returns(application);
			appharborClient.Setup(x => x.GetUser()).Returns(user);
			applicationConfiguration.Setup(x => x.SetupApplication(application.Slug, user));

			command.Run(new List<string> { application.Slug }.ToArray());

			applicationConfiguration.Verify(x => x.SetupApplication(application.Slug, user));
		}

		[Theory, AutoCommandData]
		public void ShouldThrowIfApplicationCouldntBeFound([Frozen]Mock<IApplicationConfiguration> applicationConfiguration,
			[Frozen]Mock<IAppHarborClient> appharborClient,
			LinkAppCommand command)
		{
			appharborClient.Setup(x => x.GetApplication(It.IsAny<string>())).Throws<ApiException>();

			Assert.Throws<CommandException>(() => command.Run(new List<string> { "foo" }.ToArray()));
		}
	}
}
