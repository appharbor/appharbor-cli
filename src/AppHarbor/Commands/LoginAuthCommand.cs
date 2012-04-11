using System;
using System.IO;
using RestSharp;
using RestSharp.Contrib;

namespace AppHarbor.Commands
{
	[CommandHelp("Login to AppHarbor")]
	public class LoginAuthCommand : ICommand
	{
		private readonly IAccessTokenConfiguration _accessTokenConfiguration;
		private readonly TextWriter _writer;

		public LoginAuthCommand(IAccessTokenConfiguration accessTokenConfiguration, TextWriter writer)
		{
			_accessTokenConfiguration = accessTokenConfiguration;
			_writer = writer;
		}

		public void Execute(string[] arguments)
		{
			if (_accessTokenConfiguration.GetAccessToken() != null)
			{
				throw new CommandException("You're already logged in");
			}

			_writer.Write("Username: ");
			var username = Console.ReadLine();

			_writer.Write("Password: ");
			var password = Console.ReadLine();

			var accessToken = GetAccessToken(username, password);
			_accessTokenConfiguration.SetAccessToken(accessToken);
			_writer.WriteLine("Successfully logged in as {0}", username);
		}

		public virtual string GetAccessToken(string username, string password)
		{
			//NOTE: Remove when merged into AppHarbor.NET library
			var restClient = new RestClient("https://appharbor-token-client.apphb.com");
			var request = new RestRequest("/token", Method.POST);

			request.AddParameter("username", username);
			request.AddParameter("password", password);

			var response = restClient.Execute(request);
			var accessToken = HttpUtility.ParseQueryString(response.Content)["access_token"];

			if (accessToken == null)
			{
				throw new CommandException("Couldn't log in. Try again");
			}

			return accessToken;
		}
	}
}
