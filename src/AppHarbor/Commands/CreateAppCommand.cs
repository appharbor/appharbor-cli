using System;
using System.IO;
using System.Linq;

namespace AppHarbor.Commands
{
	[CommandHelp("Create an application", "[NAME]", "create")]
	public class CreateAppCommand : ICommand
	{
		private readonly IAppHarborClient _appHarborClient;
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly TextWriter _textWriter;

		public CreateAppCommand(IAppHarborClient appHarborClient, IApplicationConfiguration applicationConfiguration, TextWriter textWriter)
		{
			_appHarborClient = appHarborClient;
			_applicationConfiguration = applicationConfiguration;
			_textWriter = textWriter;
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

			try
			{
				Console.WriteLine("This directory is already configured to track application \"{0}\".",
					_applicationConfiguration.GetApplicationId());
			}
			catch (ApplicationConfigurationException)
			{
				_applicationConfiguration.SetupApplication(result.ID, _appHarborClient.GetUser());
			}
		}
	}
}
