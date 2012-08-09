using System.IO;
using System.Linq;

namespace AppHarbor.Commands
{
	[CommandHelp("List all configuration variables")]
	public class ConfigCommand : ConsoleCommand
	{
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly IAppHarborClient _appharborClient;
		private readonly TextWriter _writer;

		public ConfigCommand(IApplicationConfiguration applicationConfiguration, IAppHarborClient appharborClient, TextWriter writer)
		{
			_applicationConfiguration = applicationConfiguration;
			_appharborClient = appharborClient;
			_writer = writer;
		}

		public override void Run(string[] arguments)
		{
			var applicationId = _applicationConfiguration.GetApplicationId();
			var configurationVariables = _appharborClient.GetConfigurationVariables(applicationId);

			if (!configurationVariables.Any())
			{
				_writer.WriteLine("No configuration variables are associated with this application");
			}

			foreach (var configurationVariable in configurationVariables)
			{
				_writer.WriteLine("{0} => {1}", configurationVariable.Key, configurationVariable.Value);
			}
		}
	}
}
