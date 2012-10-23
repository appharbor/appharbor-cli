using System;
using Castle.Windsor;

namespace AppHarbor
{
	class Program
	{
		static int Main(string[] args)
		{
			var container = new WindsorContainer()
				.Install(new AppHarborInstaller());

			var commandDispatcher = container.Resolve<CommandDispatcher>();

			try
			{
				commandDispatcher.Dispatch(args);
				return 0; // no there was no problems :)
			}
			catch (DispatchException exception)
			{
				Console.WriteLine();
				using (new ForegroundColor(ConsoleColor.Red))
				{
					Console.WriteLine("Error: {0}", exception.Message);
				}
				return 1; // yes, there was a problem :(
			}
		}
	}
}
