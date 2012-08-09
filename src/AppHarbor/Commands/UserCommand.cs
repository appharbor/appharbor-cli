using System.IO;

namespace AppHarbor.Commands
{
	[CommandHelp("Show currently logged in user")]
	public class UserCommand : ConsoleCommand
	{
		private readonly IAppHarborClient _appharborClient;
		private readonly TextWriter _writer;

		public UserCommand(IAppHarborClient appharborClient, TextWriter writer)
		{
			_appharborClient = appharborClient;
			_writer = writer;
		}

		public override void Run(string[] arguments)
		{
			var user = _appharborClient.GetUser();

			_writer.WriteLine("Username: {0}", user.Username);
			_writer.WriteLine("Email addresses: [{0}]", string.Join(" ", user.Email_Addresses));
		}
	}
}
