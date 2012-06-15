using System.IO;
using System.Linq;
using AppHarbor.Commands;
using AppHarbor.Model;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests.Commands
{
	public class CreateAppCommandTest
	{
		[Theory, AutoCommandData]
		public void ShouldThrowWhenNoArguments(CreateAppCommand command)
		{
			var exception = Assert.Throws<CommandException>(() => command.Execute(new string[0]));
			Assert.Equal("An application name must be provided to create an application", exception.Message);
		}

		[Theory, AutoCommandData]
		public void ShouldCreateApplicationWithOnlyName([Frozen]Mock<IAppHarborClient> client, CreateAppCommand command)
		{
			var arguments = new string[] { "foo" };
			VerifyArguments(client, command, arguments);
		}

		[Theory, AutoCommandData]
		public void ShouldCreateApplicationWithRegion([Frozen]Mock<IAppHarborClient> client, CreateAppCommand command, string[] arguments)
		{
			VerifyArguments(client, command, arguments);
		}

		private static void VerifyArguments(Mock<IAppHarborClient> client, CreateAppCommand command, string[] arguments)
		{
			command.Execute(arguments);
			client.Verify(x => x.CreateApplication(arguments.First(), arguments.Skip(1).FirstOrDefault()), Times.Once());
		}

		[Theory, AutoCommandData]
		public void ShouldSetupApplicationLocallyAfterCreationIfNotConfigured([Frozen]Mock<IApplicationConfiguration> applicationConfiguration, [Frozen]Mock<IAppHarborClient> client, CreateAppCommand command, CreateResult result, User user, string[] arguments)
		{
			client.Setup(x => x.CreateApplication(It.IsAny<string>(), It.IsAny<string>())).Returns(result);
			client.Setup(x => x.GetUser()).Returns(user);
			applicationConfiguration.Setup(x => x.GetApplicationId()).Throws<ApplicationConfigurationException>();

			command.Execute(arguments);
			applicationConfiguration.Verify(x => x.SetupApplication(result.Id, user), Times.Once());
		}

		[Theory, AutoCommandData]
		public void ShouldNotSetupApplicationIfAlreadyConfigured([Frozen]Mock<IApplicationConfiguration> applicationConfiguration, [Frozen]Mock<TextWriter> writer, CreateAppCommand command, string[] arguments, string applicationName)
		{
			applicationConfiguration.Setup(x => x.GetApplicationId()).Returns(applicationName);

			command.Execute(arguments);

			writer.Verify(x => x.WriteLine("This directory is already configured to track application \"{0}\".", applicationName), Times.Once());
		}

		[Theory, AutoCommandData]
		public void ShouldPrintSuccessMessageAfterCreatingApplication([Frozen]Mock<IAppHarborClient> client, [Frozen]Mock<TextWriter> writer, Mock<CreateAppCommand> command, string[] arguments, string applicationSlug)
		{
			client.Setup(x => x.CreateApplication(arguments[0], arguments[1])).Returns(new CreateResult { Id = applicationSlug });

			command.Object.Execute(arguments);

			writer.Verify(x => x.WriteLine("Created application \"{0}\" | URL: https://{0}.apphb.com", applicationSlug), Times.Once());
		}
	}
}
