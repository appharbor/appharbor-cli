using System.IO;

namespace AppHarbor.Commands
{
	public class AuthCommand : ICommand
	{
		private readonly IAppHarborClient _appharborClient;
		private readonly TextWriter _writer;

		public AuthCommand(IAppHarborClient appharborClient, TextWriter writer)
		{
			_appharborClient = appharborClient;
			_writer = writer;
		}

		public void Execute(string[] arguments)
		{
			var user = _appharborClient.GetUser();

			_writer.WriteLine("Username: {0}", user.Username);
			_writer.WriteLine("Email addresses: [{0}]", string.Join(" ", user.Email_Addresses));
		}
	}
}
