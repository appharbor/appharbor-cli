using System;
using System.Collections.Generic;
using Xunit;

namespace AppHarbor.Tests
{
	public class TypeNameMatcherTest
	{
		interface IFoo
		{
		}

		[Fact]
		public void ShouldThrowIfInitializedWithUnnasignableType()
		{
			var exception = Assert.Throws<ArgumentException>(() => new TypeNameMatcher<IFoo>(new List<Type> { typeof(string) }));
		}
	}
}
