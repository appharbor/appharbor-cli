using System;
using System.Linq;
using System.IO;

namespace AppHarbor.Commands
{
	public class CreateCommand : ICommand
	{
		private readonly IAppHarborClient _appHarborClient;
		private readonly ApplicationConfiguration _applicationConfiguration;
		private readonly IGitExecutor _gitExecutor;

		public CreateCommand(IAppHarborClient appHarborClient, ApplicationConfiguration applicationConfiguration, IGitExecutor gitExecutor)
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

			if (_gitExecutor.IsInstalled())
			{
				var user = _appHarborClient.GetUser();
				var repositoryUrl = string.Format("https://{0}@appharbor.com/{1}.git", user.Username, result.ID);

				_gitExecutor.Execute(string.Format("remote add appharbor https://{0}@appharbor.com/{1}.git", user.Username, result.ID),
					new DirectoryInfo(Directory.GetCurrentDirectory()));

				Console.WriteLine("Added \"appharbor\" as a remote repository. Push to AppHarbor with git push appharbor master");
			}
		}
	}
}
