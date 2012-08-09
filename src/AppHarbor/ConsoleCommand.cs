using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NDesk.Options;
using System.Collections.Specialized;

namespace AppHarbor
{
	public abstract class ConsoleCommand
	{
		public ConsoleCommand()
		{
			Options = new OptionSet();
			TraceCommandAfterParse = true;
			RemainingArgumentsCount = 0;
			RemainingArgumentsHelpText = "";
			RequiredOptions = new List<RequiredOptionRecord>();

			var commandType = this.GetType();
			var splitted = SplitUpperCase(commandType.Name).Where(x => x != "Command");
			Command = string.Join(" ", splitted.Reverse().ToArray()).ToLower();
			var helpAttribute = commandType.GetCustomAttributes(true).OfType<CommandHelpAttribute>().Single();
			OneLineDescription = helpAttribute.Description;
		}

		public string Command { get; private set; }
		public string OneLineDescription { get; private set; }
		public OptionSet Options { get; protected set; }
		public bool TraceCommandAfterParse { get; private set; }
		public int? RemainingArgumentsCount { get; private set; }
		public string RemainingArgumentsHelpText { get; private set; }
		public List<RequiredOptionRecord> RequiredOptions { get; private set; }

		public ConsoleCommand HasAdditionalArguments(int? count = 0, string helpText = "")
		{
			RemainingArgumentsCount = count;
			RemainingArgumentsHelpText = helpText;
			return this;
		}

		public ConsoleCommand AllowsAnyAdditionalArguments(string helpText = "")
		{
			HasAdditionalArguments(null, helpText);
			return this;
		}

		public ConsoleCommand SkipsCommandSummaryBeforeRunning()
		{
			TraceCommandAfterParse = false;
			return this;
		}

		public ConsoleCommand HasOption(string prototype, string description, Action<string> action)
		{
			Options.Add(prototype, description, action);

			return this;
		}

		public ConsoleCommand HasRequiredOption(string prototype, string description, Action<string> action)
		{
			var requiredRecord = new RequiredOptionRecord();

			var previousOptions = Options.ToArray();

			Options.Add(prototype, description, s =>
			{
				requiredRecord.WasIncluded = true;
				action(s);
			});

			var newOption = Options.Single(o => !previousOptions.Contains(o));

			requiredRecord.Name = newOption.GetNames().OrderByDescending(n => n.Length).First();

			RequiredOptions.Add(requiredRecord);

			return this;
		}

		public ConsoleCommand HasOption<T>(string prototype, string description, Action<T> action)
		{
			Options.Add(prototype, description, action);
			return this;
		}

		public ConsoleCommand HasOption(string prototype, string description, OptionAction<string, string> action)
		{
			Options.Add(prototype, description, action);
			return this;
		}

		public ConsoleCommand HasOption<TKey, TValue>(string prototype, string description, OptionAction<TKey, TValue> action)
		{
			Options.Add(prototype, description, action);
			return this;
		}

		public abstract void Run(string[] remainingArguments);

		private static string[] SplitUpperCase(string source)
		{
			if (source == null)
				return new string[] { }; //Return empty array.

			if (source.Length == 0)
				return new string[] { "" };

			StringCollection words = new StringCollection();
			int wordStartIndex = 0;

			char[] letters = source.ToCharArray();
			// Skip the first letter. we don't care what case it is.
			for (int i = 1; i < letters.Length; i++)
			{
				if (char.IsUpper(letters[i]))
				{
					//Grab everything before the current index.
					words.Add(new String(letters, wordStartIndex, i - wordStartIndex));
					wordStartIndex = i;
				}
			}

			//We need to have the last word.
			words.Add(new String(letters, wordStartIndex, letters.Length - wordStartIndex));

			//Copy to a string array.
			string[] wordArray = new string[words.Count];
			words.CopyTo(wordArray, 0);
			return wordArray;
		}
	}
}