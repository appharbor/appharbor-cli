using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;
using NDesk.Options;

namespace AppHarbor
{
	public class CommandDispatcher
	{
		private readonly IAliasMatcher _aliasMatcher;
		private readonly ITypeNameMatcher _typeNameMatcher;
		private readonly IKernel _kernel;
		private readonly ConsoleHelper _consoleHelper;

		public CommandDispatcher(IAliasMatcher aliasMatcher, ITypeNameMatcher typeNameMatcher, IKernel kernel, ConsoleHelper consoleHelper)
		{
			_aliasMatcher = aliasMatcher;
			_typeNameMatcher = typeNameMatcher;
			_kernel = kernel;
			_consoleHelper = consoleHelper;
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

			var consoleWriter = Console.Out;
			ConsoleCommand command = null;
			try
			{
				command = (ConsoleCommand)_kernel.Resolve(matchingType);

				var remainingArguments = command.Options.Parse(args.Skip(argsToSkip));
				CheckRequiredArguments(command);

				if (command.RemainingArgumentsCount > 0)
				{
					CheckRemainingArguments(remainingArguments, (int)command.RemainingArgumentsCount);
				}

				_consoleHelper.ShowParsedCommand(command);

				command.Run(remainingArguments.ToArray());
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
				if (!ConsoleHelpAsException.WriterErrorMessage(exception, consoleWriter))
				{
					throw new DispatchException();
				}
				consoleWriter.WriteLine();
				if (exception is ConsoleHelpAsException || exception is OptionException)
				{
					_consoleHelper.ShowCommandHelp(command);
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
