using System.Diagnostics;

namespace AppHarbor.Commands
{
	[CommandHelp("Open application on appharbor.com", alias: "open")]
	public class OpenAppCommand : ApplicationCommand
	{
		public OpenAppCommand(IApplicationConfiguration applicationConfiguration)
			: base(applicationConfiguration)
		{
		}

		protected override void InnerExecute(string[] arguments)
		{
			Process.Start(string.Format("https://appharbor.com/applications/{0}", ApplicationId));
		}
	}
}
