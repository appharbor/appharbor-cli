using System;
using System.Linq;

namespace AppHarbor.Commands
{
	public class CreateCommand : ICommand
	{
		private readonly IAppHarborClient _appHarborClient;
		private readonly ApplicationConfiguration _applicationConfiguration;
		private readonly GitExecutor _gitExecutor;

		public CreateCommand(IAppHarborClient appHarborClient, ApplicationConfiguration applicationConfiguration, GitExecutor gitExecutor)
		{
			_appHarborClient = appHarborClient;
			_applicationConfiguration = applicationConfiguration;
			_gitExecutor = gitExecutor;
		}

		public void Execute(string[] arguments)
		{
			if (arguments.Length == 0)
			{
				throw new CommandException("An application name must be provided to create an application");
			}

			var result = _appHarborClient.CreateApplication(arguments.First(), arguments.Skip(1).FirstOrDefault());

			Console.WriteLine("Created application \"{0}\" | URL: https://{0}.apphb.com", result.ID);
		}
	}
}
