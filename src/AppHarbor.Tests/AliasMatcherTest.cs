using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests
{
	public class AliasMatcherTest
	{
		[CommandHelp("foo", options: "", alias: "bar")]
		class Foo { }

		[Fact]
		public void ShouldThrowIfInitializedWithTypesHavingNoCommandHelpAttribute()
		{
			Assert.Throws<ArgumentException>(() => new AliasMatcher(new List<Type> { typeof(string) }));
		}

		[Theory]
		[InlineData("bar")]
		[InlineData("Bar")]
		public void ShouldMatchTypeDecoratedWithAlias(string command)
		{
			var aliasMatcher = new AliasMatcher(new List<Type> { typeof(Foo) });
			Assert.Equal(typeof(Foo), aliasMatcher.GetMatchedType(command));
		}

		[Fact]
		public void ShouldNotMatchTypeWithoutMatchingAlias()
		{
			var aliasMatcher = new AliasMatcher(new List<Type> { typeof(Foo) });
			Assert.Throws<ArgumentException>(() => aliasMatcher.GetMatchedType("baz"));
		}
	}
}
