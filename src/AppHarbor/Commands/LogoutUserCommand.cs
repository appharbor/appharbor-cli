using System.IO;

namespace AppHarbor.Commands
{
	[CommandHelp("Logout of AppHarbor", alias: "logout")]
	public class LogoutUserCommand : Command
	{
		private readonly IAccessTokenConfiguration _accessTokenConfiguration;
		private readonly TextWriter _writer;

		public LogoutUserCommand(IAccessTokenConfiguration accessTokenConfiguration, TextWriter writer)
		{
			_accessTokenConfiguration = accessTokenConfiguration;
			_writer = writer;
		}

		protected override void InnerExecute(string[] arguments)
		{
			_accessTokenConfiguration.DeleteAccessToken();
			_writer.WriteLine("Successfully logged out.");
		}
	}
}
