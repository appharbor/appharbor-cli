using AppHarbor.Commands;
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
	}
}
