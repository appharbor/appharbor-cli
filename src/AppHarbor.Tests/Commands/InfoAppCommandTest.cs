using System.IO;
using AppHarbor.Commands;
using AppHarbor.Model;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests.Commands
{
	public class InfoAppCommandTest
	{
		[Theory, AutoCommandData]
		public void ShouldReturnApplicationInformation([Frozen]Mock<TextWriter> writer, [Frozen]Mock<IAppHarborClient> client, [Frozen]Application application, InfoAppCommand command)
		{
			client.Setup(x => x.GetApplication(It.IsAny<string>())).Returns(application);
			command.Execute(new string[0]);

			writer.Verify(x => x.WriteLine("Name: {0}", application.Name));
			writer.Verify(x => x.WriteLine("Id: {0}", application.Slug));
			writer.Verify(x => x.WriteLine("Region: {0}", application.RegionIdentifier));
		}
	}
}
