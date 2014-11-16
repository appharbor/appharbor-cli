using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using System.IO;

namespace AppHarbor.Commands
{
	[CommandHelp("Show log drains and tokens")]
	public class DrainCommand : ApplicationCommand
	{
		private const string AppHarborBaseUrl = "https://appharbor.com/";

		private readonly string _accessToken;
		private readonly IRestClient _restClient;
		private readonly TextWriter _writer;

		public DrainCommand(IApplicationConfiguration applicationConfiguration, IAccessTokenConfiguration accessTokenConfiguration, TextWriter writer)
			: base(applicationConfiguration)
		{
			_accessToken = accessTokenConfiguration.GetAccessToken();
			_restClient = new RestClient(AppHarborBaseUrl);
			_writer = writer;
		}

		protected override void InnerExecute(string[] arguments)
		{
			var request = new RestRequest("applications/{slug}/drains", Method.GET)
				.AddUrlSegment("slug", ApplicationId)
				.AddHeader("Authorization", string.Format("BEARER {0}", _accessToken));
			request.RequestFormat = DataFormat.Json;

			var drains = _restClient.Execute<List<Drain>>(request);

			if (!drains.Data.Any())
			{
				_writer.WriteLine("No drains are associated with the application.");
			}

			foreach (var hostname in drains.Data)
			{
				_writer.WriteLine(string.Format("{0} ({1})", hostname.Value, hostname.Token));
			}
		}

		private class Drain
		{
			public string Url { get; set; }
			public string Token { get; set; }
			public string Value { get; set; }
		}
	}
}
