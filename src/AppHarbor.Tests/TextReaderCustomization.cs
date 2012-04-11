using System.IO;
using Moq;
using Ploeh.AutoFixture;

namespace AppHarbor.Tests
{
	public class TextReaderCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			var textReaderMock = new Mock<TextReader>();
			fixture.Customize<TextReader>(x => x.FromFactory(() => { return textReaderMock.Object; }));
			fixture.Customize<Mock<TextReader>>(x => x.FromFactory(() => { return textReaderMock; }));
		}
	}
}
