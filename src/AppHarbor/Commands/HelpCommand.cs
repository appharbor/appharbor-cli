using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace AppHarbor.Commands
{
	[CommandHelp("Display help summary")]
	public class HelpCommand : ICommand
	{
		private readonly IEnumerable<Type> _commandTypes;
		private readonly TextWriter _writer;

		public HelpCommand(IEnumerable<Type> commandTypes, TextWriter writer)
		{
			_commandTypes = commandTypes;
			_writer = writer;
		}

		public void Execute(string[] arguments)
		{
			_writer.WriteLine("Usage: appharbor COMMAND [command-options]");
			_writer.WriteLine("");

			_writer.WriteLine("Available commands:");
			_writer.WriteLine("");

			foreach (var commandType in _commandTypes.Where(x => x.IsClass)
				.OrderBy(x => GetScope(x))
				.ThenBy(x => x.Name))
			{
				var usageStringBuilder = new StringBuilder();

				var splitted = SplitUpperCase(commandType.Name).Where(x => x != "Command");
				usageStringBuilder.Append("  ");
				usageStringBuilder.Append(string.Join(" ", splitted.Reverse()));
				var helpAttribute = commandType.GetCustomAttributes(true).OfType<CommandHelpAttribute>().Single();
					usageStringBuilder.Append(string.Format(" {0}", helpAttribute.Options));

				while (usageStringBuilder.Length < 30)
				{
					usageStringBuilder.Append(" ");
				}

				_writer.Write(usageStringBuilder.ToString().ToLower());
				_writer.Write(string.Concat("#  ", helpAttribute.Description));

				if (!string.IsNullOrEmpty(helpAttribute.Alias))
				{
					_writer.Write(" (alias: \"{0}\")", helpAttribute.Alias);
				}
				_writer.WriteLine();
			}
		}

		private static string GetScope(Type x)
		{
			return SplitUpperCase(x.Name).Where(y => y != "Command").Last();
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
