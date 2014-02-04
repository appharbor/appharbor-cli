using System.IO;
using RestSharp;
using RestSharp.Contrib;
using System;

namespace AppHarbor.Commands
{
	[CommandHelp("Login to AppHarbor", alias: "login")]
	public class LoginUserCommand : Command
	{
		private readonly IAccessTokenConfiguration _accessTokenConfiguration;
		private readonly IMaskedInput _maskedInput;
		private readonly TextReader _reader;
		private readonly TextWriter _writer;

		public LoginUserCommand(IAccessTokenConfiguration accessTokenConfiguration, IMaskedInput maskedInput, TextReader reader, TextWriter writer)
		{
			_accessTokenConfiguration = accessTokenConfiguration;
			_maskedInput = maskedInput;
			_reader = reader;
			_writer = writer;
		}

		protected override void InnerExecute(string[] arguments)
		{
			if (_accessTokenConfiguration.GetAccessToken() != null)
			{
				throw new CommandException("You're already logged in. You need to log out (\"logout\") before you can log in again.");
			}

			string username = null;
			string password = null;

			if (arguments == null || arguments.Length != 2)
			{
				_writer.WriteLine("Please supply your Credentials.");
				_writer.Write("Username: ");
				username = _reader.ReadLine();

				_writer.Write("Password: ");
				password = _maskedInput.Get();
				_writer.WriteLine();                
			}
			else
			{
				_writer.WriteLine("Using parameterized Credentials.");
				username = arguments[0];
				password = arguments[1];
			}

			if (String.IsNullOrEmpty(username))
			{
				_writer.WriteLine("No Username supplied, login failed.");
				return;
			}
            if (String.IsNullOrEmpty(password))
			{
				_writer.WriteLine("No Password supplied, login failed.");
			}            

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
