using System.IO;

namespace AppHarbor.Commands
{
	[CommandHelp("Show currently logged in user")]
	public class UserCommand : Command
	{
		private readonly IAppHarborClient _appharborClient;
		private readonly TextWriter _writer;

		public UserCommand(IAppHarborClient appharborClient, TextWriter writer)
		{
			_appharborClient = appharborClient;
			_writer = writer;
		}

		protected override void InnerExecute(string[] arguments)
		{
			var user = _appharborClient.GetUser();

			_writer.WriteLine("Username: {0}", user.Username);
			_writer.WriteLine("Email addresses: [{0}]", string.Join(" ", user.Email_Addresses));
		}
	}
}
