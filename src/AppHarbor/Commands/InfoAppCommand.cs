using System.IO;

namespace AppHarbor.Commands
{
	[CommandHelp("Get application details")]
	public class InfoAppCommand : ApplicationCommand
	{
		private readonly IAppHarborClient _client;
		private readonly TextWriter _writer;

		public InfoAppCommand(IAppHarborClient client, IApplicationConfiguration applicationConfiguration, TextWriter writer)
			: base(applicationConfiguration)
		{
			_client = client;
			_writer = writer;
		}

		protected override void InnerExecute(string[] arguments)
		{
			var application = _client.GetApplication(ApplicationId);

			_writer.WriteLine("Id: {0}", application.Slug);
			_writer.WriteLine("Name: {0}", application.Name);
			_writer.WriteLine("Region: {0}", application.RegionIdentifier);
		}
	}
}
