using System.Diagnostics;

namespace AppHarbor.Commands
{
	[CommandHelp("Open application on appharbor.com", alias: "open")]
	public class OpenAppCommand : ICommand
	{
		private readonly IApplicationConfiguration _applicationConfiguration;

		public OpenAppCommand(IApplicationConfiguration applicationConfiguration)
		{
			_applicationConfiguration = applicationConfiguration;
		}

		public void Execute(string[] arguments)
		{
			Process.Start(string.Format("https://appharbor.com/applications/{0}", _applicationConfiguration.GetApplicationId()));
		}
	}
}
