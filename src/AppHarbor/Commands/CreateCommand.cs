using System;
using System.Linq;

namespace AppHarbor.Commands
{
	[CommandHelp("Create an application", "[NAME]")]
	public class CreateCommand : ICommand
	{
		private readonly IAppHarborClient _appHarborClient;
		private readonly IApplicationConfiguration _applicationConfiguration;

		public CreateCommand(IAppHarborClient appHarborClient, IApplicationConfiguration applicationConfiguration)
		{
			_appHarborClient = appHarborClient;
			_applicationConfiguration = applicationConfiguration;
		}

		public void Execute(string[] arguments)
		{
			if (arguments.Length == 0)
			{
				throw new CommandException("An application name must be provided to create an application");
			}

			var result = _appHarborClient.CreateApplication(arguments.First(), arguments.Skip(1).FirstOrDefault());

			Console.WriteLine("Created application \"{0}\" | URL: https://{0}.apphb.com", result.ID);
			Console.WriteLine("");

			_applicationConfiguration.SetupApplication(result.ID, _appHarborClient.GetUser());
		}
	}
}
