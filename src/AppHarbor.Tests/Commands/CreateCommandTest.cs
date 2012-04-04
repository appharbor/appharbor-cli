using System;
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
	public class CreateCommandTest
	{
		[Theory, AutoCommandData]
		public void ShouldThrowWhenNoArguments(CreateCommand command)
		{
			var exception = Assert.Throws<CommandException>(() => command.Execute(new string[0]));
			Assert.Equal("An application name must be provided to create an application", exception.Message);
		}

		[Theory, AutoCommandData]
		public void ShouldCreateApplicationWithOnlyName([Frozen]Mock<IAppHarborClient> client, CreateCommand command)
		{
			var arguments = new string[] { "foo" };
			VerifyArguments(client, command, arguments);
		}

		[Theory, AutoCommandData]
		public void ShouldCreateApplicationWithRegion([Frozen]Mock<IAppHarborClient> client, CreateCommand command, string[] arguments)
		{
			VerifyArguments(client, command, arguments);
		}

		private static void VerifyArguments(Mock<IAppHarborClient> client, CreateCommand command, string[] arguments)
		{
			command.Execute(arguments);
			client.Verify(x => x.CreateApplication(arguments.First(), arguments.Skip(1).FirstOrDefault()), Times.Once());
		}

		[Theory, AutoCommandData]
		public void ShouldPrintSuccessMessageAfterCreatingApplication([Frozen]Mock<IAppHarborClient> client, Mock<CreateCommand> command, string[] arguments, string applicationSlug)
		{
			client.Setup(x => x.CreateApplication(arguments[0], arguments[1])).Returns(new CreateResult<string> { ID = applicationSlug });
			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);
				command.Object.Execute(arguments);

				Assert.Equal(string.Format(string.Concat("Created application \"{0}\" | URL: https://{0}.apphb.com", Environment.NewLine), applicationSlug), writer.ToString());
			}
		}

		[Theory, AutoCommandData]
		public void ShouldNotSetupGitIfNotInstalled([Frozen]Mock<IGitExecutor> gitExecutor, CreateCommand command, string[] arguments)
		{
			command.Execute(arguments);

			gitExecutor.Verify(x => x.Execute(It.IsAny<string>(), It.IsAny<DirectoryInfo>()), Times.Never());
			gitExecutor.Verify(x => x.IsInstalled(), Times.Once());
		}
	}
}
