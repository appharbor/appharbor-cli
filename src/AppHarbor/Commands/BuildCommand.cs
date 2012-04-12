using System;
using System.IO;
using System.Linq;

namespace AppHarbor.Commands
{
	[CommandHelp("List latest builds")]
	public class BuildCommand : ICommand
	{
		public const string OutputFormat = "{0,-43}{1,-13}{2,5}";

		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly IAppHarborClient _appharborClient;
		private readonly TextWriter _writer;

		public BuildCommand(IApplicationConfiguration applicationConfiguration, IAppHarborClient appharborClient, TextWriter writer)
		{
			_applicationConfiguration = applicationConfiguration;
			_appharborClient = appharborClient;
			_writer = writer;
		}

		public void Execute(string[] arguments)
		{
			var applicationId = _applicationConfiguration.GetApplicationId();
			var builds = _appharborClient.GetBuilds(applicationId);

			_writer.WriteLine(string.Format(OutputFormat, "Commit", "Status", "Deployed"));

			if (!builds.Any())
			{
				_writer.WriteLine("No builds are associated with this application.");
			}

			foreach (var build in builds)
			{
				var commitId = GetShortened(build.Commit.ID, 7);

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
