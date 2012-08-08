using System.Collections.Generic;
using System.IO;
using AppHarbor.Commands;
using AppHarbor.Model;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests.Commands
{
	public class AppCommandTest
	{
		[Theory, AutoCommandData]
		public void ShouldWriteApplicationsToConsole([Frozen]Mock<IAppHarborClient> client, Mock<TextWriter> writer, List<Application> applications, AppCommand command)
		{
			applications.Add(new Application { Slug = "foo" });
			applications.Add(new Application { Slug = "bar" });

			client.Setup(x => x.GetApplications()).Returns(applications);

			command.Run(new string[0]);

			foreach (var application in applications)
			{
				writer.Verify(x => x.WriteLine(application.Slug), Times.Once());
			}
		}
	}
}
