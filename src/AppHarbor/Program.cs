using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
				Console.WriteLine("Error: {0}", exception.Message);
			}
			catch (ApiException exception)
			{
				Console.WriteLine("An error occured while connecting to AppHarbor.");
			}
		}
	}
}
