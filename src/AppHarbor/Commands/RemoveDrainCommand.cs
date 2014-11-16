using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RestSharp;

namespace AppHarbor.Commands
{
	[CommandHelp("Remove a log drain", "[DRAIN URL]")]
	public class RemoveDrainCommand : ApplicationCommand
	{
		private const string AppHarborBaseUrl = "https://appharbor.com/";

		private readonly string _accessToken;
		private readonly IRestClient _restClient;

		public RemoveDrainCommand(IApplicationConfiguration applicationConfiguration, IAccessTokenConfiguration accessTokenConfiguration)
			: base(applicationConfiguration)
		{
			_accessToken = accessTokenConfiguration.GetAccessToken();
			_restClient = new RestClient(AppHarborBaseUrl);
		}

		protected override void InnerExecute(string[] arguments)
		{
			var request = new RestRequest("applications/{slug}/drains", Method.GET)
				.AddUrlSegment("slug", ApplicationId)
				.AddHeader("Authorization", string.Format("BEARER {0}", _accessToken));
			request.RequestFormat = DataFormat.Json;

			var drains = _restClient.Execute<List<Drain>>(request);

			var drain = drains.Data.FirstOrDefault(x => x.Value == arguments[0]);

			if (drain == null)
			{
				throw new CommandException("No drains with that URL is associated with this application");
			}

			var url = new Uri(drain.Url);
			var destroyRequest = new RestRequest(url, Method.DELETE)
			{
				RequestFormat = DataFormat.Json,
			}
			.AddHeader("Authorization", string.Format("BEARER {0}", _accessToken));
			var destroyResponse = _restClient.Execute(destroyRequest);

			if (destroyResponse.StatusCode != HttpStatusCode.NoContent)
			{
				throw new CommandException("Couldn't remove drain");
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
