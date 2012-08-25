using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;

namespace AppHarbor.Tests
{
	public class AutoCommandDataAttribute : AutoDataAttribute
	{
		public AutoCommandDataAttribute()
			: base(new Fixture().Customize(new DomainCustomization()))
		{
		}
	}
}
