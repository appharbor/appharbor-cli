using System;
using System.IO;

namespace AppHarbor.Commands
{
	[CommandHelp("Unlink application from directory", alias: "unlink")]
	public class UnlinkAppCommand : Command
	{
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly TextWriter _writer;

		public UnlinkAppCommand(IApplicationConfiguration applicationConfiguration, TextWriter writer)
		{
			_applicationConfiguration = applicationConfiguration;
			_writer = writer;
		}

		protected override void InnerExecute(string[] arguments)
		{
			_applicationConfiguration.RemoveConfiguration();
			_writer.WriteLine("Successfully unlinked directory.");
		}
	}
}
