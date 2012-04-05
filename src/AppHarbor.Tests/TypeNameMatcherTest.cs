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

		[Fact]
		public void ShouldThrowIfInitializedWithUnnasignableType()
		{
			var exception = Assert.Throws<ArgumentException>(() => new TypeNameMatcher<IFoo>(new List<Type> { typeof(string) }));
		}

		[Theory]
		[InlineData("Foo", typeof(Foo))]
		[InlineData("foo", typeof(Foo))]
		public void ShouldGetTypeStartingWithCommandName(string commandName, Type fooType)
		{
			var matcher = new TypeNameMatcher<IFoo>(new Type[] { fooType });
			Assert.Equal(fooType, matcher.GetMatchedType(commandName));
		}
	}
}
