using System.IO;
using System.Linq;

namespace AppHarbor.Commands
{
	[CommandHelp("Create an application", "[NAME]", "create")]
	public class CreateAppCommand : Command
	{
		private readonly IAppHarborClient _appHarborClient;
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly TextWriter _textWriter;

		private string _region;

		public CreateAppCommand(IAppHarborClient appHarborClient, IApplicationConfiguration applicationConfiguration, TextWriter textWriter)
		{
			_appHarborClient = appHarborClient;
			_applicationConfiguration = applicationConfiguration;
			_textWriter = textWriter;

			OptionSet.Add("r|region=", "Optionally specify a region", x => _region = x);
		}

		protected override void InnerExecute(string[] arguments)
		{
			if (arguments.Length == 0)
			{
				throw new CommandException("An application name must be provided to create an application");
			}

			var result = _appHarborClient.CreateApplication(arguments.First(), _region);

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
