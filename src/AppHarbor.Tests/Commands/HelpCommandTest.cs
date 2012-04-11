using System;
using System.Collections.Generic;
using System.IO;
using AppHarbor.Commands;
using Xunit;

namespace AppHarbor.Tests.Commands
{
	public class HelpCommandTest
	{
		private static Type FooCommandType = typeof(FooCommand);
		private static Type FooBarCommandType = typeof(FooBarCommand);
		private static Type BazQuxCommandType = typeof(BazQuxCommand);

		[CommandHelp("Lorem Ipsum motherfucker", "[bar]")]
		class FooCommand { }

		[CommandHelp("Lorem Ipsum motherfucker", "[bar]")]
		class FooBarCommand { }

		[CommandHelp("Ipsum lol", "[Quz]", "quux")]
		class BazQuxCommand { }

		[Fact]
		public void ShouldOutputHelpInformation()
		{
			var types = new List<Type> { FooCommandType, BazQuxCommandType, FooBarCommandType };
			var writer = new StringWriter();
			var helpCommand = new HelpCommand(types, writer);

			helpCommand.Execute(new string[0]);

			Assert.Equal("Usage: appharbor COMMAND [command-options]\r\n\r\nAvailable commands:\r\n\r\n  bar foo [bar]          #  Lorem Ipsum motherfucker\r\n  foo [bar]              #  Lorem Ipsum motherfucker\r\n  qux baz [quz]          #  Ipsum lol (alias: \"quux\")\r\n", writer.ToString());
		}
	}
}
