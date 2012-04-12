using System.Collections.Generic;
using System.IO;
using AppHarbor.Commands;
using AppHarbor.Model;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests.Commands
{
	public class HostnameCommandTest
	{
		[Theory, AutoCommandData]
		public void ShouldOutputHostnames([Frozen]Mock<IApplicationConfiguration> applicationConfiguration,
			[Frozen]Mock<IAppHarborClient> client,
			[Frozen]Mock<TextWriter> writer,
			HostnameCommand command, string applicationId)
		{
			applicationConfiguration.Setup(x => x.GetApplicationId()).Returns(applicationId);
			var hostnames = new List<Hostname>
			{
				new Hostname { Value = "http://example.com", Canonical = true },
				new Hostname { Value = "http://*.example.com", Canonical = false },
			};

			client.Setup(x => x.GetHostnames(applicationId)).Returns(hostnames);

			command.Execute(new string[0]);

			foreach (var hostname in hostnames)
			{
				var output = hostname.Value;
				output += hostname.Canonical ? " (canonical)" : "";

				writer.Verify(x => x.WriteLine(output));
			}
		}
	}
}
