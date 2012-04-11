using System;
using System.Linq;
using Castle.MicroKernel;

namespace AppHarbor
{
	public class CommandDispatcher
	{
		private readonly IAliasMatcher _aliasMatcher;
		private readonly ITypeNameMatcher _typeNameMatcher;
		private readonly IKernel _kernel;

		public CommandDispatcher(IAliasMatcher aliasMatcher, ITypeNameMatcher typeNameMatcher, IKernel kernel)
		{
			_aliasMatcher = aliasMatcher;
			_typeNameMatcher = typeNameMatcher;
			_kernel = kernel;
		}

		public void Dispatch(string[] args)
		{
			var commandArgument = args.Any() ? string.Concat(args.Skip(1).FirstOrDefault(), args[0]) : "help";
			Type matchingType = null;
			int argsToSkip = 0;

			if (_typeNameMatcher.IsSatisfiedBy(commandArgument))
			{
				matchingType = _typeNameMatcher.GetMatchedType(commandArgument);
				argsToSkip = 2;
			}
			else if (_aliasMatcher.IsSatisfiedBy(args[0]))
			{
				matchingType = _aliasMatcher.GetMatchedType(args[0]);
				argsToSkip = 1;
			}
			else
			{
				throw new DispatchException(string.Format("The command \"{0}\" doesn't match a command name or alias", string.Join(" ", args)));
			}

			var command = (ICommand)_kernel.Resolve(matchingType);

			try
			{
				command.Execute(args.Skip(argsToSkip).ToArray());
			}
			catch (CommandException exception)
			{
				throw new DispatchException(exception.Message);
			}
		}
	}
}
