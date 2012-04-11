using System.IO;
using Moq;
using Ploeh.AutoFixture;

namespace AppHarbor.Tests
{
	public class TextWriterCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			var textWriterMock = new Mock<TextWriter>();
			fixture.Customize<TextWriter>(x => x.FromFactory(() => { return textWriterMock.Object; }));
			fixture.Customize<Mock<TextWriter>>(x => x.FromFactory(() => { return textWriterMock; }));
		}
	}
}
