using System;
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
			throw new NotImplementedException();
		}
	}
}
