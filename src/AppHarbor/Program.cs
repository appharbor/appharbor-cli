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

			if (args.Any())
			{
				commandDispatcher.Dispatch(args);
			}

			Console.WriteLine("Usage: appharbor COMMAND [command-options]");
			Console.WriteLine("");

			Console.WriteLine("Help topics (type \"appharbor help TOPIC\"):");
		}
	}
}
