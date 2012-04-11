using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace AppHarbor.Tests
{
	public class DomainCustomization : CompositeCustomization
	{
		public DomainCustomization()
			: base(
				new AutoMoqCustomization(),
				new TextReaderCustomization(),
				new TextWriterCustomization())
		{
		}
	}
}
