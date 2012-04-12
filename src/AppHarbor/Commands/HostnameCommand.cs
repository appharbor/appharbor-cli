using System.IO;
using System.Linq;

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

			if (!hostnames.Any())
			{
				_writer.WriteLine("No hostnames are associated with the application.");
			}

			foreach (var hostname in hostnames)
			{
				var output = hostname.Value;
				output += hostname.Canonical ? " (canonical)" : "";

				_writer.WriteLine(output);
			}
		}
	}
}
