﻿using System;
using System.IO;
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
			var commandArguments = args.TakeWhile(x => !x.StartsWith("-")).Take(2);
			var commandTypeNameCandidate = commandArguments.Any() ? string.Concat(commandArguments.Skip(1).FirstOrDefault(), args[0]) : "help";

			Type matchingType = null;
			int argsToSkip = 0;

			if (_typeNameMatcher.IsSatisfiedBy(commandTypeNameCandidate))
			{
				matchingType = _typeNameMatcher.GetMatchedType(commandTypeNameCandidate);
				argsToSkip = commandArguments.Count();
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

			var command = (Command)_kernel.Resolve(matchingType);

			try
			{
				command.Execute(args.Skip(argsToSkip).ToArray());
			}
			catch (ApiException exception)
			{
				throw new DispatchException(string.Format("An error occured while connecting to the API. {0}", exception.Message));
			}
			catch (CommandException exception)
			{
				command.WriteUsage(invokedWith: string.Join(" ", commandArguments),
					writer: _kernel.Resolve<TextWriter>());

				if (!(exception is HelpException))
				{
					throw new DispatchException(exception.Message);
				}
			}
		}
	}
}
