using System;
using Castle.Windsor;

namespace AppHarbor
{
	class Program
	{
		static void Main(string[] args)
		{
			var container = new WindsorContainer()
				.Install(new AppHarborInstaller());

			var commandDispatcher = container.Resolve<CommandDispatcher>();

			try
			{
				commandDispatcher.Dispatch(args);
			}
			catch (DispatchException exception)
			{
				Console.WriteLine();
				using (new ForegroundColor(ConsoleColor.Red))
				{
					Console.WriteLine("Error: {0}", exception.Message);
				}
			}
		}
	}
}
