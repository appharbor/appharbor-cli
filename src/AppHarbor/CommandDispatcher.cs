using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;

namespace AppHarbor
{
	public class CommandDispatcher
	{
		private readonly IEnumerable<Type> _candidateTypes;
		private readonly IKernel _kernel;

		public CommandDispatcher(IEnumerable<Type> candidateTypes, IKernel kernel)
		{
			_candidateTypes = candidateTypes;
			_kernel = kernel;
		}

		public void Dispatch(string[] args)
		{
			var commandTypes = _candidateTypes.Where(x => typeof(ICommand).IsAssignableFrom(x));

			var commandName = args[0];
			var matchingType = commandTypes.FirstOrDefault(x =>
				typeof(ICommand).IsAssignableFrom(x) && x.Name.ToLower().StartsWith(commandName.ToLower()));

			if (matchingType == null)
			{
				throw new ArgumentException(string.Format("The command \"{0}\" does not exist", commandName));
			}

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
