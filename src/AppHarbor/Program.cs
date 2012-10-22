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
                // no there was no problems :)
                return 0;
			}
			catch (DispatchException exception)
			{
				Console.WriteLine();
				using (new ForegroundColor(ConsoleColor.Red))
				{
					Console.WriteLine("Error: {0}", exception.Message);
				}
                // yes, there was a problem :(
                return 1;
			}
		}
	}
}
