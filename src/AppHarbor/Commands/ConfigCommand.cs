using System.IO;
using System.Linq;

namespace AppHarbor.Commands
{
	[CommandHelp("List all configuration variables")]
	public class ConfigCommand : ApplicationCommand
	{
		private readonly IAppHarborClient _appharborClient;
		private readonly TextWriter _writer;

		public ConfigCommand(IApplicationConfiguration applicationConfiguration, IAppHarborClient appharborClient, TextWriter writer)
			: base(applicationConfiguration)
		{
			_appharborClient = appharborClient;
			_writer = writer;
		}

		protected override void InnerExecute(string[] arguments)
		{
			var configurationVariables = _appharborClient.GetConfigurationVariables(ApplicationId);

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
