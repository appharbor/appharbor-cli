using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppHarbor.Commands;
using AppHarbor.Model;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests.Commands
{
	public class AppCommandTest
	{
		[Theory, AutoCommandData]
		public void ShouldWriteApplicationsToConsole([Frozen]Mock<IAppHarborClient> client, List<Application> applications, AppCommand command)
		{
			applications.Add(new Application { Slug = "foo" });
			applications.Add(new Application { Slug = "bar" });

			client.Setup(x => x.GetApplications()).Returns(applications);
			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);
				command.Execute(new string[0]);

				Assert.Equal(string.Concat(applications.Select(x => string.Concat(x.Slug, Environment.NewLine))), writer.ToString());
			}
		}
	}
}
