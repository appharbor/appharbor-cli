using System;

namespace AppHarbor.Commands
{
	[CommandHelp("List your applications")]
	public class AppCommand : ICommand
	{
		private readonly IAppHarborClient _client;

		public AppCommand(IAppHarborClient appharborClient)
		{
			_client = appharborClient;
		}

		public void Execute(string[] arguments)
		{
			var applications = _client.GetApplications();
			foreach (var application in applications)
			{
				Console.WriteLine(application.Slug);
			}
		}
	}
}
