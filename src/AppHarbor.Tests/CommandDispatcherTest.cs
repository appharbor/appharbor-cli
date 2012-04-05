using System.Linq;
using Castle.MicroKernel;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests
{
	public class CommandDispatcherTest
	{
		public class FooCommand : ICommand
		{
			public virtual void Execute(string[] arguments)
			{
			}
		}

		[Theory]
		[InlineAutoCommandData("foo", "foo", null)]
		[InlineAutoCommandData("foo:bar", "foo", "bar")]
		public void ShouldDispatchCommandWithASingleParameter(
			string argument,
			string commandName,
			string scope,
			[Frozen]Mock<ITypeNameMatcher> typeNameMatcher,
			[Frozen]Mock<IKernel> kernel,
			Mock<FooCommand> command,
			CommandDispatcher commandDispatcher)
		{
			var commandType = typeof(FooCommand);
			typeNameMatcher.Setup(x => x.GetMatchedType(commandName, scope)).Returns(commandType);

			kernel.Setup(x => x.Resolve(commandType)).Returns(command.Object);

			var dispatchArguments = new string[] { argument };
			commandDispatcher.Dispatch(dispatchArguments);

			command.Verify(x => x.Execute(It.Is<string[]>(y => !y.Any())));
		}
	}
}
