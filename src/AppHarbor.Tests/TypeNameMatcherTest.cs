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

		class Foo : IFoo { }

		private static Type FooType = typeof(Foo);

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
			var matcher = new TypeNameMatcher<IFoo>(new Type[] { FooType });
			Assert.Equal(FooType, matcher.GetMatchedType(commandName));
		}

		[Theory]
		[InlineData("Bar")]
		public void ShouldThrowWhenNoTypesMatches(string commandName)
		{
			var matcher = new TypeNameMatcher<IFoo>(new Type[] { FooType });
			Assert.Throws<ArgumentException>(() => matcher.GetMatchedType(commandName));
		}
	}
}
