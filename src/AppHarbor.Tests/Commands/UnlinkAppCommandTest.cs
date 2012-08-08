using System.IO;
using AppHarbor.Commands;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests.Commands
{
	public class UnlinkAppCommandTest
	{
		[Theory, AutoCommandData]
		public void ShouldUnlinkApplication([Frozen]Mock<IApplicationConfiguration> applicationConfiguration,
			[Frozen]Mock<TextWriter> writer, UnlinkAppCommand command)
		{
			command.Run(new string[0]);

			applicationConfiguration.Verify(x => x.RemoveConfiguration());
			writer.Verify(x => x.WriteLine("Successfully unlinked directory."));
		}
	}
}
