using System.Collections.Generic;
using AppHarbor.Commands;
using AppHarbor.Model;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests.Commands
{
	public class LinkCommandTest
	{
		[Theory, AutoCommandData]
		public void ShouldThrowIfNoArguments(LinkCommand command)
		{
			Assert.Throws<CommandException>(() => command.Execute(new string[0]));
		}

		[Theory, AutoCommandData]
		public void ShouldSetupApplication([Frozen]Mock<IApplicationConfiguration> applicationConfiguration,
			[Frozen]Mock<IAppHarborClient> appharborClient,
			LinkCommand command,
			Application application,
			User user)
		{
			appharborClient.Setup(x => x.GetApplication(application.Slug)).Returns(application);
			appharborClient.Setup(x => x.GetUser()).Returns(user);
			applicationConfiguration.Setup(x => x.SetupApplication(application.Slug, user));

			command.Execute(new List<string> { application.Slug }.ToArray());

			applicationConfiguration.Verify(x => x.SetupApplication(application.Slug, user));
		}

		[Theory, AutoCommandData]
		public void ShouldThrowIfApplicationCouldntBeFound([Frozen]Mock<IApplicationConfiguration> applicationConfiguration,
			[Frozen]Mock<IAppHarborClient> appharborClient,
			LinkCommand command)
		{
			appharborClient.Setup(x => x.GetApplication(It.IsAny<string>())).Throws<ApiException>();

			Assert.Throws<CommandException>(() => command.Execute(new List<string> { "foo" }.ToArray()));
		}
	}
}
