using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;
using Moq;
using Xunit;
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
		[PropertyData("FooArguments")]
		public void ShouldParseAndExecuteCommandWithArguments(string[] commands)
		{
			var kernelMock = new Mock<IKernel>();
			var fooCommandType = typeof(FooCommand);
			var commandDispatcher = new CommandDispatcher(new Type[] { fooCommandType }, kernelMock.Object );

			var commandMock = new Mock<FooCommand>();
			kernelMock.Setup(x => x.Resolve(It.Is<Type>(y => y == fooCommandType))).Returns(commandMock.Object);

			commandDispatcher.Dispatch(commands);

			commandMock.Verify(x => x.Execute(
				It.Is<string[]>(y => ArraysEqual(y, commands.Skip(1).ToArray()))), Times.Once());

		}

		[Theory]
		[InlineAutoCommandData("bar")]
		[InlineAutoCommandData("foobar")]
		public void ShouldNotMatchCommandThatDoesntExist(string commandName)
		{
			var kernelMock = new Mock<IKernel>();
			var fooCommandType = typeof(FooCommand);
			var commandDispatcher = new CommandDispatcher(new Type[] { fooCommandType }, kernelMock.Object );

			var exception = Assert.Throws<ArgumentException>(() => commandDispatcher.Dispatch(new string[] { commandName }));
			Assert.Equal(string.Format("The command \"{0}\" does not exist", commandName), exception.Message);
		}


		public static IEnumerable<object[]> FooArguments
		{
			get
			{
				yield return new object[] { new string[] { "foo" } };
				yield return new object[] { new string[] { "Foo" } };
				yield return new object[] { new string[] { "foo", "bar" } };
			}
		}

		private static bool ArraysEqual<T>(T[] a1, T[] a2)
		{
			if (ReferenceEquals(a1, a2))
			{
				return true;
			}

			if (a1 == null || a2 == null)
			{
				return false;
			}

			if (a1.Length != a2.Length)
			{
				return false;
			}

			var comparer = EqualityComparer<T>.Default;
			for (int i = 0; i < a1.Length; i++)
			{
				if (!comparer.Equals(a1[i], a2[i]))
				{
					return false;
				}
			}
			return true;
		}
	}
}
