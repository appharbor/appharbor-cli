using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;

namespace AppHarbor
{
	public class CommandDispatcher
	{
		private readonly ITypeNameMatcher _typeNameMatcher;
		private readonly IKernel _kernel;

		public CommandDispatcher(ITypeNameMatcher typeNameMatcher, IKernel kernel)
		{
			_typeNameMatcher = typeNameMatcher;
			_kernel = kernel;
		}

		public void Dispatch(string[] args)
		{
			var matchingType = _typeNameMatcher.GetMatchedType(args.Any() ? args[0] : "help");

			var command = (ICommand)_kernel.Resolve(matchingType);

			try
			{
				command.Execute(args.Skip(1).ToArray());
			}
			catch (CommandException exception)
			{
				Console.WriteLine(string.Format("Error: {0}", exception.Message));
			}
		}
	}
}
