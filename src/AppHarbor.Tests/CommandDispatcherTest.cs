using System;
using System.Linq;
using Castle.MicroKernel;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests
{
	public class CommandDispatcherTest
	{
		private static Type FooCommandType = typeof(FooCommand);

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
			typeNameMatcher.Setup(x => x.GetMatchedType("help")).Returns(FooCommandType);
			kernel.Setup(x => x.Resolve(FooCommandType)).Returns(command.Object);

			commandDispatcher.Dispatch(new string[0]);

			typeNameMatcher.VerifyAll();
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
			typeNameMatcher.Setup(x => x.GetMatchedType(argument)).Returns(FooCommandType);

			kernel.Setup(x => x.Resolve(FooCommandType)).Returns(command.Object);

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
			typeNameMatcher.Setup(x => x.GetMatchedType(argument.Split().First())).Returns(FooCommandType);
			kernel.Setup(x => x.Resolve(FooCommandType)).Returns(command.Object);

			commandDispatcher.Dispatch(argument.Split());

			command.Verify(x => x.Execute(It.Is<string[]>(y => y.Length == 1 && y.Any(z => z == commandArgument))));
		}
	}
}
