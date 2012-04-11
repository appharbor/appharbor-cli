using System.IO;

namespace AppHarbor.Commands
{
	[CommandHelp("Logout of AppHarbor", alias: "logout")]
	public class LogoutAuthCommand : ICommand
	{
		private readonly IAccessTokenConfiguration _accessTokenConfiguration;
		private readonly TextWriter _writer;

		public LogoutAuthCommand(IAccessTokenConfiguration accessTokenConfiguration, TextWriter writer)
		{
			_accessTokenConfiguration = accessTokenConfiguration;
			_writer = writer;
		}

		public void Execute(string[] arguments)
		{
			_accessTokenConfiguration.DeleteAccessToken();
			_writer.WriteLine("Successfully logged out.");
		}
	}
}
