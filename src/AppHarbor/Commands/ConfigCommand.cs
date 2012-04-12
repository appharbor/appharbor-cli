using System.IO;

namespace AppHarbor.Commands
{
	public class ConfigCommand : ICommand
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

		public void Execute(string[] arguments)
		{
			var applicationId = _applicationConfiguration.GetApplicationId();

			foreach (var configurationVariable in _appharborClient.GetConfigurationVariables(applicationId))
			{
				_writer.WriteLine("{0} => {1}", configurationVariable.Key, configurationVariable.Value);
			}
		}
	}
}
