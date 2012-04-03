using System.Linq;
using AppHarbor.Commands;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests.Commands
{
	public class CreateCommandTest
	{
		private readonly IFixture _fixture;

		public CreateCommandTest()
		{
			_fixture = new Fixture().Customize(new AutoMoqCustomization());
		}
		[Theory, AutoCommandData]
		public void ShouldCreateApplication([Frozen]Mock<IAppHarborClient> client, CreateCommand command, string[] arguments)
		{
			command.Execute(arguments);

			client.Verify(x => x.CreateApplication(arguments.First(), arguments.Skip(1).FirstOrDefault()), Times.Once());
		}
	}
}
