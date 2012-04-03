using AppHarbor.Commands;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Xunit;

namespace AppHarbor.Tests.Commands
{
	public class CreateCommandTest
	{
		private readonly IFixture _fixture;

		public CreateCommandTest()
		{
			_fixture = new Fixture().Customize(new AutoMoqCustomization());
		}

		[Fact]
		public void ShouldCreateApplication()
		{
			var client = _fixture.Freeze<Mock<IAppHarborClient>>();
			var command = _fixture.CreateAnonymous<CreateCommand>();

			command.Execute(new string[] { "foo", "var" });

			client.Verify(x => x.CreateApplication("bar", "baz"), Times.Once());
		}
	}
}
