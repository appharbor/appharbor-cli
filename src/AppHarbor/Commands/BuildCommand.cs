using System;
using System.IO;
using System.Linq;

namespace AppHarbor.Commands
{
	[CommandHelp("List latest builds")]
	public class BuildCommand : ApplicationCommand
	{
		public const string OutputFormat = "{0,-43}{1,-13}{2,5}";

		private readonly IAppHarborClient _appharborClient;
		private readonly TextWriter _writer;

		public BuildCommand(IApplicationConfiguration applicationConfiguration, IAppHarborClient appharborClient, TextWriter writer)
			: base(applicationConfiguration)
		{
			_appharborClient = appharborClient;
			_writer = writer;
		}

		protected override void InnerExecute(string[] arguments)
		{
			var builds = _appharborClient.GetBuilds(ApplicationId);

			_writer.WriteLine(string.Format(OutputFormat, "Commit", "Status", "Deployed"));

			if (!builds.Any())
			{
				_writer.WriteLine("No builds are associated with this application.");
			}

			foreach (var build in builds)
			{
				var commitId = GetShortened(build.Commit.Id, 7);

				var message = GetShortened(string.Concat(commitId, " | ", build.Commit.Message.Trim('\n')), 40, "...");
				var buildOutput = String.Format(OutputFormat, message, build.Status, build.Deployed);

				_writer.WriteLine(buildOutput);
			}
		}

		private static string GetShortened(string input, int width, string continuedString = "")
		{
			return input.Length > width ? input.Substring(0, width - continuedString.Length) + continuedString : input;
		}
	}
}
