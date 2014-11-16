using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using RestSharp;

namespace AppHarbor.Commands
{
	[CommandHelp("Get application log", alias: "log")]
	public class LogAppCommand : ApplicationCommand
	{
		private const string AppHarborBaseUrl = "https://appharbor.com/";

		private static IList<ConsoleColor> Colors = new List<ConsoleColor>
		{
			ConsoleColor.Red,
			ConsoleColor.Green,
			ConsoleColor.Yellow,
			ConsoleColor.Magenta,
			ConsoleColor.Cyan
		};

		private readonly string _accessToken;
		private readonly TextWriter _writer;
		private readonly IRestClient _restClient;
		private readonly IDictionary<string, ConsoleColor> _assignedColors;

		private bool _tail;
		private int _limit = 15;
		private string _processFilter;
		private string _sourceFilter;

		public LogAppCommand(IApplicationConfiguration applicationConfiguration, IAccessTokenConfiguration accessTokenConfiguration, TextWriter writer)
			: base(applicationConfiguration)
		{
			_accessToken = accessTokenConfiguration.GetAccessToken();
			_writer = writer;
			_restClient = new RestClient(AppHarborBaseUrl);
			_assignedColors = new Dictionary<string, ConsoleColor>();

			OptionSet.Add("t|tail", "Tail log", x => _tail = true);
			OptionSet.Add("n|num=", "Number of log messages", (int x) => _limit = x);
			OptionSet.Add("p|process=", "Filter log to this process (case-sensitive)", x => _processFilter = x);
			OptionSet.Add("s|source=", "Filter log to this source", x => _sourceFilter = x);
		}

		protected override void  InnerExecute(string[] arguments)
		{
			var request = new RestRequest("applications/{slug}/logsession", Method.POST)
			{
				RequestFormat = DataFormat.Json
			}
				.AddUrlSegment("slug", ApplicationId)
				.AddHeader("Authorization", string.Format("BEARER {0}", _accessToken))
				.AddBody(new
				{
					tail = _tail,
					limit = _limit - 1,
					sourcefilter = _sourceFilter,
					processFilter = _processFilter,
				});

			var response = _restClient.Execute(request);
			var locationHeader = response.Headers.FirstOrDefault(x =>
				string.Equals(x.Name, "location",StringComparison.InvariantCultureIgnoreCase));
			if (response.StatusCode != HttpStatusCode.Created || locationHeader.Value == null)
			{
				throw new CommandException("Couldn't create log session. Try again later.");
			}

			var sessionUrl = new Uri(locationHeader.Value.ToString());
			try
			{
				using (var webClient = new WebClient())
				{
					using (var stream = webClient.OpenRead(sessionUrl))
					using (var reader = new StreamReader(stream))
					{
						while (true)
						{
							var line = reader.ReadLine();
							if (line != null)
							{
								WriteColorizedLine(line);
							}

							if (reader.EndOfStream)
							{
								break;
							}
						}
					}
				}
			}
			catch (WebException exception)
			{
				Console.WriteLine("Log session ended with message: {0}", exception.Message);
			}
		}

		private void WriteColorizedLine(string line)
		{
			var parsed = Regex.Match(line, @"^(.*?\[([\w-]+)([\d\.]+)?\]:)(.*)?$");
			var defaultColor = Console.ForegroundColor;

			var lineIdentifier = parsed.Groups[2].Value;
			if (!_assignedColors.ContainsKey(lineIdentifier))
			{
				_assignedColors[lineIdentifier] = Colors[_assignedColors.Count() % Colors.Count()];
			}

			Console.ForegroundColor = _assignedColors[lineIdentifier];
			Console.Write(parsed.Groups[1].Value);
			Console.ForegroundColor = defaultColor;
			Console.WriteLine(parsed.Groups[4].Value);
		}
	}
}
