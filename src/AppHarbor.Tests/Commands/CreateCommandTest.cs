using System;
using System.Collections.Generic;
using System.Linq;
using AppHarbor.Commands;
using Moq;
using Ploeh.AutoFixture;
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
		public void ShouldCreateApplicationWithOnlyName([Frozen]Mock<IAppHarborClient> client, CreateCommand command, Fixture fixture)
		{
			var arguments = new string[] { "foo" };
			command.Execute(arguments);

			client.Verify(x => x.CreateApplication(arguments.First(), arguments.Skip(1).FirstOrDefault()), Times.Once());
		}

		[Theory, AutoCommandData]
		public void ShouldCreateApplicationWithRegion([Frozen]Mock<IAppHarborClient> client, CreateCommand command, string[] arguments)
		{
			command.Execute(arguments);

			client.Verify(x => x.CreateApplication(arguments.First(), arguments.Skip(1).FirstOrDefault()), Times.Once());
		}
	}
}
