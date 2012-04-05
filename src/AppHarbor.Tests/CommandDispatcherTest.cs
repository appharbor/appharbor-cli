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

		[Theory, AutoCommandData]
		public void ShouldDispatchHelpWhenNoCommand(
			[Frozen]Mock<ITypeNameMatcher> typeNameMatcher,
			[Frozen]Mock<IKernel> kernel,
			Mock<FooCommand> command,
			CommandDispatcher commandDispatcher)
		{
			var commandType = typeof(FooCommand);
			typeNameMatcher.Setup(x => x.GetMatchedType("help")).Returns(commandType);
			kernel.Setup(x => x.Resolve(commandType)).Returns(command.Object);

			commandDispatcher.Dispatch(new string[0]);
		}

		[Theory]
		[InlineAutoCommandData("foo")]
		[InlineAutoCommandData("foo:bar")]
		public void ShouldDispatchCommandWithoutParameters(
			string argument,
			[Frozen]Mock<ITypeNameMatcher> typeNameMatcher,
			[Frozen]Mock<IKernel> kernel,
			Mock<FooCommand> command,
			CommandDispatcher commandDispatcher)
		{
			var commandType = typeof(FooCommand);
			typeNameMatcher.Setup(x => x.GetMatchedType(argument)).Returns(commandType);

			kernel.Setup(x => x.Resolve(commandType)).Returns(command.Object);

			var dispatchArguments = new string[] { argument };
			commandDispatcher.Dispatch(dispatchArguments);

			command.Verify(x => x.Execute(It.Is<string[]>(y => !y.Any())));
		}

		[Theory]
		[InlineAutoCommandData("foo:bar baz", "baz")]
		[InlineAutoCommandData("foo baz", "baz")]
		public void ShouldDispatchCommandWithParameter(
			string argument,
			string commandArgument,
			[Frozen]Mock<ITypeNameMatcher> typeNameMatcher,
			[Frozen]Mock<IKernel> kernel,
			Mock<FooCommand> command,
			CommandDispatcher commandDispatcher)
		{
			var commandType = typeof(FooCommand);
			typeNameMatcher.Setup(x => x.GetMatchedType(argument.Split().First())).Returns(commandType);
			kernel.Setup(x => x.Resolve(commandType)).Returns(command.Object);

			commandDispatcher.Dispatch(argument.Split());

			command.Verify(x => x.Execute(It.Is<string[]>(y => y.Length == 1 && y.Any(z => z == commandArgument))));
		}
	}
}
