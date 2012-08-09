using System;
using System.Linq;
using Castle.MicroKernel;
using System.Collections.Generic;
using NDesk.Options;

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

			var console = Console.Out;
			ConsoleCommand command = null;
			try
			{
				command = (ConsoleCommand)_kernel.Resolve(matchingType);

				CheckRequiredArguments(command);

				var remainingArguments = args.Skip(argsToSkip).ToArray();

				if (command.RemainingArgumentsCount > 0)
				{
					CheckRemainingArguments(remainingArguments, (int)command.RemainingArgumentsCount);
				}

				ConsoleHelp.ShowParsedCommand(command, console);

				command.Run(remainingArguments);
			}
			catch (ApiException exception)
			{
				throw new DispatchException(string.Format("An error occured while connecting to the API. {0}", exception.Message));
			}
			catch (CommandException exception)
			{
				throw new DispatchException(exception.Message);
			}
			catch (Exception exception)
			{
				if (!ConsoleHelpAsException.WriterErrorMessage(exception, console))
				{
					throw new DispatchException();
				}
				console.WriteLine();
				if (exception is ConsoleHelpAsException || exception is OptionException)
				{
					ConsoleHelp.ShowCommandHelp(command, console);
				}
			}
		}

		private static void CheckRequiredArguments(ConsoleCommand command)
		{
			var missingOptions = command.RequiredOptions
				.Where(o => !o.WasIncluded).Select(o => o.Name).OrderBy(n => n).ToArray();

			if (missingOptions.Any())
			{
				throw new ConsoleHelpAsException("Missing option: " + string.Join(", ", missingOptions));
			}
		}

		private static void CheckRemainingArguments(IList<string> remainingArguments, int parametersRequiredAfterOptions)
		{
			if (remainingArguments.Count() < parametersRequiredAfterOptions)
			{
				throw new ConsoleHelpAsException(
					string.Format("Invalid number of arguments-- expected {0} more.", parametersRequiredAfterOptions - remainingArguments.Count()));
			}

			if (remainingArguments.Count() > parametersRequiredAfterOptions)
			{
				throw new ConsoleHelpAsException("Extra parameters specified: " + string.Join(", ", remainingArguments.Skip(parametersRequiredAfterOptions).ToArray()));
			}
		}

	}
}
