using System;
using System.Collections.Generic;
using Moq;
using Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests
{
	public class AliasMatcherTest
	{
		[CommandHelp("foo", alias: "bar")]
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

		[Theory, AutoCommandData]
		public void ShouldBeSatisfiedWhenTypeIsReturned(Mock<AliasMatcher> matcher)
		{
			matcher.Setup(x => x.GetMatchedType(It.IsAny<string>())).Returns(typeof(string));
			Assert.True(matcher.Object.IsSatisfiedBy("foo"));
		}

		[Theory, AutoCommandData]
		public void ShouldNotBeSatisfiedWhenTypeCantBeFound(Mock<AliasMatcher> matcher)
		{
			matcher.Setup(x => x.GetMatchedType(It.IsAny<string>())).Throws<ArgumentException>();
			Assert.False(matcher.Object.IsSatisfiedBy("foo"));
		}
	}
}
