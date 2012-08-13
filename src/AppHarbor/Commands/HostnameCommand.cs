using System.IO;
using System.Linq;

namespace AppHarbor.Commands
{
	[CommandHelp("List all associated hostnames")]
	public class HostnameCommand : ApplicationCommand
	{
		private readonly IAppHarborClient _appharborClient;
		private readonly TextWriter _writer;

		public HostnameCommand(IApplicationConfiguration applicationConfiguration, IAppHarborClient appharborClient, TextWriter writer)
			: base(applicationConfiguration)
		{
			_appharborClient = appharborClient;
			_writer = writer;
		}

		protected override void InnerExecute(string[] arguments)
		{
			var hostnames = _appharborClient.GetHostnames(ApplicationId);

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
