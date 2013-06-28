using System.IO;
using RestSharp;
using RestSharp.Contrib;

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

			_writer.Write("Username: ");
			var username = _reader.ReadLine();

			_writer.Write("Password: ");
			var password = _maskedInput.Get();
			_writer.WriteLine();

			var accessToken = GetAccessToken(username, password);
			_accessTokenConfiguration.SetAccessToken(accessToken);
			_writer.WriteLine("Successfully logged in as {0}", username);
		}

		public virtual string GetAccessToken(string username, string password)
		{
			return AccessTokenHelper.GetAccessToken(username, password);
		}

	}
}
