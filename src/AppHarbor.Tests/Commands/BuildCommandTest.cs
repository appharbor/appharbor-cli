﻿using System;
using System.Collections.Generic;
using System.IO;
using AppHarbor.Commands;
using AppHarbor.Model;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests.Commands
{
	public class BuildCommandTest
	{
		[Theory, AutoCommandData]
		public void ShouldOutputBuilds([Frozen]Mock<IApplicationConfiguration> applicationConfiguration,
			[Frozen]Mock<IAppHarborClient> client,
			[Frozen]Mock<TextWriter> writer,
			BuildCommand command, string applicationId)
		{
			applicationConfiguration.Setup(x => x.GetApplicationId()).Returns(applicationId);
			var builds = new List<Build>
			{
				new Build { Commit = new Commit { Message = "foo bar", Id = "baz" }, Status = "Failed", Deployed = DateTime.Now },
			};

			client.Setup(x => x.GetBuilds(applicationId)).Returns(builds);

			Assert.DoesNotThrow(() => command.Execute(new string[0]));
		}

		[Theory, AutoCommandData]
		public void ShouldWriteIfNoBuildsExists(BuildCommand command,
			[Frozen]Mock<TextWriter> writer, string applicationId)
		{
			command.Execute(new string[0]);

			writer.Verify(x => x.WriteLine("No builds are associated with this application."));
		}
	}
}
