using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace AppHarbor.Commands
{
	[CommandHelp("Display help summary")]
	public class HelpCommand : ICommand
	{
		private readonly IEnumerable<Type> _commandTypes;

		public HelpCommand(IEnumerable<Type> commandTypes)
		{
			_commandTypes = commandTypes;
		}

		public void Execute(string[] arguments)
		{
			Console.WriteLine("Usage: appharbor COMMAND [command-options]");
			Console.WriteLine("");

			Console.WriteLine("Available commands:");
			Console.WriteLine("");

			foreach (var commandType in _commandTypes.Where(x => x.IsClass)
				.OrderBy(x => Reverse(x.Name))
				.Reverse()
				.OrderBy(x => x.Name.Length)
				.OrderBy(x => x.Name))
			{
				var usageStringBuilder = new StringBuilder();

				var splitted = SplitUpperCase(commandType.Name).Where(x => x != "Command");
				usageStringBuilder.Append("  ");
				usageStringBuilder.Append(string.Join(":", splitted.Reverse()));
				var helpAttribute = commandType.GetCustomAttributes(true).OfType<CommandHelpAttribute>().Single();
					usageStringBuilder.Append(string.Format(" {0}", helpAttribute.Options));

				while (usageStringBuilder.Length < 25)
				{
					usageStringBuilder.Append(" ");
				}

				Console.Write(usageStringBuilder.ToString().ToLower());
				Console.Write(string.Concat("#  ", helpAttribute.Description));
				Console.WriteLine();
			}
		}

		private static string Reverse(string sz)
		{
			if (string.IsNullOrEmpty(sz) || sz.Length == 1)
			{
				return sz;
			}

			var chars = sz.ToCharArray();
			Array.Reverse(chars);

			return new string(chars);
		}

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
