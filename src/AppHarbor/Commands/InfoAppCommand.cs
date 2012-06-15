using System.IO;

namespace AppHarbor.Commands
{
	[CommandHelp("Get application details")]
	public class InfoAppCommand : ICommand
	{
		private readonly IAppHarborClient _client;
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly TextWriter _writer;

		public InfoAppCommand(IAppHarborClient client, IApplicationConfiguration applicationConfiguration, TextWriter writer)
		{
			_client = client;
			_applicationConfiguration = applicationConfiguration;
			_writer = writer;
		}

		public void Execute(string[] arguments)
		{
			var id = _applicationConfiguration.GetApplicationId();
			var application = _client.GetApplication(id);

			_writer.WriteLine("Id: {0}", application.Slug);
			_writer.WriteLine("Name: {0}", application.Name);
			_writer.WriteLine("Region: {0}", application.RegionIdentifier);
		}
	}
}
