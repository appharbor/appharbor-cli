using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests
{
	public class TypeNameMatcherTest
	{
		interface IFoo
		{
		}

		class FooCommand : IFoo { }

		class FooBarCommand : IFoo { }

		class FooBazCommand : IFoo { }

		private static Type FooCommandType = typeof(FooCommand);
		private static Type FooBarCommandType = typeof(FooBarCommand);
		private static Type FooBazCommandType = typeof(FooBazCommand);

		[Fact]
		public void ShouldThrowIfInitializedWithUnnasignableType()
		{
			var exception = Assert.Throws<ArgumentException>(() => new TypeNameMatcher<IFoo>(new List<Type> { typeof(string) }));
		}

		[Theory]
		[InlineData("Foo")]
		[InlineData("foo")]
		public void ShouldGetTypeStartingWithCommandName(string commandName)
		{
			var matcher = new TypeNameMatcher<IFoo>(new Type[] { FooCommandType });
			Assert.Equal(FooCommandType, matcher.GetMatchedType(commandName, null));
		}

		[Theory]
		[InlineData("Foo")]
		public void ShouldThrowWhenMoreThanOneTypeMatches(string commandName)
		{
			var matcher = new TypeNameMatcher<IFoo>(new Type[] { FooBazCommandType, FooBarCommandType });

			var exception = Assert.Throws<ArgumentException>(() => matcher.GetMatchedType(commandName, null));
			Assert.Equal(string.Format("More than one command matches \"{0}\".", commandName), exception.Message);
		}

		[Theory]
		[InlineData("Bar")]
		public void ShouldThrowWhenNoTypesMatches(string commandName)
		{
			var matcher = new TypeNameMatcher<IFoo>(new Type[] { FooCommandType });

			var exception = Assert.Throws<ArgumentException>(() => matcher.GetMatchedType(commandName, null));
			Assert.Equal(string.Format("No commands matches \"{0}\".", commandName), exception.Message);
		}

		[Theory]
		[InlineData("bar")]
		[InlineData("Bar")]
		public void ShouldReturnScopedCommand(string scope)
		{
			var matcher = new TypeNameMatcher<IFoo>(new Type[] { FooCommandType, FooBarCommandType });
			matcher.GetMatchedType("foo", scope);
		}
	}
}
