using System.Collections.Generic;
using System.IO;
using AppHarbor.Commands;
using AppHarbor.Model;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests.Commands
{
	public class ConfigCommandTest
	{
		[Theory, AutoCommandData]
		public void ShouldAddConfigurationVariables(string arguments,
			[Frozen]Mock<IApplicationConfiguration> applicationConfiguration,
			[Frozen]Mock<IAppHarborClient> client,
			[Frozen]Mock<TextWriter> writer,
			ConfigCommand command, string applicationId)
		{
			applicationConfiguration.Setup(x => x.GetApplicationId()).Returns(applicationId);
			var configurationVariables = new List<ConfigurationVariable>
			{
				new ConfigurationVariable { Key = "foo", Value = "bar" },
				new ConfigurationVariable { Key = "baz", Value = "qux" },
			};

			client.Setup(x => x.GetConfigurationVariables(applicationId)).Returns(configurationVariables);

			command.Execute(new string[0]);

			foreach (var configurationVariable in configurationVariables)
			{
				writer.Verify(x => x.WriteLine("{0} => {1}", configurationVariable.Key, configurationVariable.Value));
			}
		}
	}
}
