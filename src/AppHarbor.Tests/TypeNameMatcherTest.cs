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

		private static Type FooCommandType = typeof(FooCommand);

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
			var matcher = new TypeNameMatcher<IFoo>(new Type[] { FooCommandType, FooCommandType });
			Assert.Throws<ArgumentException>(() => matcher.GetMatchedType(commandName, null));
		}

		[Theory]
		[InlineData("Bar")]
		public void ShouldThrowWhenNoTypesMatches(string commandName)
		{
			var matcher = new TypeNameMatcher<IFoo>(new Type[] { FooCommandType });
			Assert.Throws<ArgumentException>(() => matcher.GetMatchedType(commandName, null));
		}
	}
}
