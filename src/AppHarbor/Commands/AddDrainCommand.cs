using System.Net;
using RestSharp;

namespace AppHarbor.Commands
{
	[CommandHelp("Add a log drain", "[DRAIN_URL]")]
	public class AddDrainCommand : ApplicationCommand
	{
		private readonly string _accessToken;
		private readonly IRestClient _restClient;

		public AddDrainCommand(IApplicationConfiguration applicationConfiguration, IAccessTokenConfiguration accessTokenConfiguration)
			: base(applicationConfiguration)
		{
			_accessToken = accessTokenConfiguration.GetAccessToken();
			_restClient = new RestClient("https://appharbor.com/");
		}

		protected override void  InnerExecute(string[] arguments)
		{
			var request = new RestRequest("applications/{slug}/drains", Method.POST)
			{
				RequestFormat = DataFormat.Json
			}
				.AddUrlSegment("slug", ApplicationId)
				.AddHeader("Authorization", string.Format("BEARER {0}", _accessToken))
				.AddBody(new { url = arguments[0] });

			var response = _restClient.Execute(request);

			if (response.StatusCode != HttpStatusCode.Created)
			{
				throw new CommandException(response.Content);
			}
		}
	}
}
