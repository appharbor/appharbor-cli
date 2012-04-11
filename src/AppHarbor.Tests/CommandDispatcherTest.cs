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
			var helpArgument = "help";
			typeNameMatcher.Setup(x => x.IsSatisfiedBy(helpArgument)).Returns(true);
			typeNameMatcher.Setup(x => x.GetMatchedType(helpArgument)).Returns(FooCommandType);
			kernel.Setup(x => x.Resolve(FooCommandType)).Returns(command.Object);

			commandDispatcher.Dispatch(new string[0]);

			typeNameMatcher.VerifyAll();
		}

		[Theory]
		[InlineAutoCommandData("barfoo", "foo", "bar")]
		public void ShouldDispatchCommandWithoutParameters(
			string commandArgument,
			string scope,
			string commandName,
			[Frozen]Mock<ITypeNameMatcher> typeNameMatcher,
			[Frozen]Mock<IKernel> kernel,
			Mock<FooCommand> command,
			CommandDispatcher commandDispatcher)
		{
			typeNameMatcher.Setup(x => x.IsSatisfiedBy(commandArgument)).Returns(true);
			typeNameMatcher.Setup(x => x.GetMatchedType(commandArgument)).Returns(FooCommandType);
			kernel.Setup(x => x.Resolve(FooCommandType)).Returns(command.Object);

			commandDispatcher.Dispatch(new string[] { scope, commandName });

			command.Verify(x => x.Execute(It.Is<string[]>(y => !y.Any())));
		}

		[Theory]
		[InlineAutoCommandData("barfoo", "foo", "bar", "baz")]
		public void ShouldDispatchCommandWithParameter(
			string commandArgument,
			string scope,
			string commandName,
			string commandParameter,
			[Frozen]Mock<ITypeNameMatcher> typeNameMatcher,
			[Frozen]Mock<IKernel> kernel,
			Mock<FooCommand> command,
			CommandDispatcher commandDispatcher)
		{
			typeNameMatcher.Setup(x => x.IsSatisfiedBy(commandArgument)).Returns(true);
			typeNameMatcher.Setup(x => x.GetMatchedType(commandArgument)).Returns(FooCommandType);
			kernel.Setup(x => x.Resolve(FooCommandType)).Returns(command.Object);

			commandDispatcher.Dispatch(new string[] { scope, commandName, commandParameter });

			command.Verify(x => x.Execute(It.Is<string[]>(y => y.Length == 1 && y.Any(z => z == commandParameter))));
		}
	}
}
