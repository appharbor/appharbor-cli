using AppHarbor.Commands;
using Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests.Commands
{
	public class RemoveConfigCommandTest
	{
		[Theory, AutoCommandData]
		public void ShouldThrowIfNoArguments(RemoveConfigCommand command)
		{
			Assert.Throws<CommandException>(() => command.Execute(new string[0]));
		}
	}
}
