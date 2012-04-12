using System.IO;

namespace AppHarbor.Commands
{
	public class HostnameCommand : ICommand
	{
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly IAppHarborClient _appharborClient;
		private readonly TextWriter _writer;

		public HostnameCommand(IApplicationConfiguration applicationConfiguration, IAppHarborClient appharborClient, TextWriter writer)
		{
			_applicationConfiguration = applicationConfiguration;
			_appharborClient = appharborClient;
			_writer = writer;
		}

		public void Execute(string[] arguments)
		{
			var applicationId = _applicationConfiguration.GetApplicationId();
			var hostnames = _appharborClient.GetHostnames(applicationId);

			foreach (var hostname in hostnames)
			{
				var output = hostname.Value;
				output += hostname.Canonical ? " (canonical)" : "";

				_writer.WriteLine(output);
			}
		}
	}
}
