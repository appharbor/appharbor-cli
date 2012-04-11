using System;
using System.IO;
using Ploeh.AutoFixture;

namespace AppHarbor.Tests
{
	public class TextWriterCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			fixture.Customize<TextWriter>(x => x.FromFactory(() => { return Console.Out; }));
		}
	}
}
