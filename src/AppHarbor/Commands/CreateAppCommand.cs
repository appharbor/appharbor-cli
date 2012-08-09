using System.IO;
using System.Linq;
using NDesk.Options;

namespace AppHarbor.Commands
{
	[CommandHelp("Create an application", alias: "create", numberOfAdditionalArguments: 1, additionalArgumentsHelp:  "<NAME>")]
	public class CreateAppCommand : ConsoleCommand
	{
		private readonly IAppHarborClient _appHarborClient;
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly TextWriter _textWriter;

		public CreateAppCommand(IAppHarborClient appHarborClient, IApplicationConfiguration applicationConfiguration, TextWriter textWriter)
		{
			_appHarborClient = appHarborClient;
			_applicationConfiguration = applicationConfiguration;
			_textWriter = textWriter;

			HasOption("r|region:", "Optional String argument which is null if no value follow is specified", x => Region = x);
		}

		public string Region
		{
			get;
			set;
		}

		public override void Run(string[] arguments)
		{
			var result = _appHarborClient.CreateApplication(arguments.First(), Region);

			_textWriter.WriteLine("Created application \"{0}\" | URL: https://{0}.apphb.com", result.Id);
			_textWriter.WriteLine("");

			try
			{
				_textWriter.WriteLine("This directory is already configured to track application \"{0}\".",
					_applicationConfiguration.GetApplicationId());
			}
			catch (ApplicationConfigurationException)
			{
				_applicationConfiguration.SetupApplication(result.Id, _appHarborClient.GetUser());
			}
		}
	}
}
