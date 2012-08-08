using System.IO;
using System.Linq;

namespace AppHarbor.Commands
{
	[CommandHelp("List your applications")]
	public class AppCommand : ICommand
	{
		private readonly IAppHarborClient _client;
		private readonly TextWriter _writer;

		public AppCommand(IAppHarborClient appharborClient, TextWriter writer)
		{
			_client = appharborClient;
			_writer = writer;
		}

		public void Run(string[] arguments)
		{
			var applications = _client.GetApplications();
			foreach (var application in applications.OrderBy(x => x.Slug))
			{
				_writer.WriteLine(application.Slug);
			}
		}
	}
}
