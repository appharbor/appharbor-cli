using Ploeh.AutoFixture.Xunit;

namespace AppHarbor.Tests
{
	public class InlineAutoCommandDataAttribute : InlineAutoDataAttribute
	{
		public InlineAutoCommandDataAttribute(params object[] values)
			: base(new AutoCommandDataAttribute(), values)
		{
		}
	}
}
