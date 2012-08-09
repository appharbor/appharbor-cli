using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace AppHarbor
{
	public class ConsoleHelper
	{
		private readonly TextWriter _writer;

		public ConsoleHelper(TextWriter writer)
		{
			_writer = writer;
		}

		public void ShowCommandHelp(ConsoleCommand selectedCommand)
		{
			var haveOptions = selectedCommand.Options.Count > 0;

			_writer.WriteLine("'" + selectedCommand.Command + "' - " + selectedCommand.OneLineDescription);
			_writer.WriteLine();
			_writer.Write("Expected usage: {0} {1} ", AppDomain.CurrentDomain.FriendlyName, selectedCommand.Command);

			if (haveOptions)
			{
				_writer.Write("<options> ");
			}

			_writer.WriteLine(selectedCommand.RemainingArgumentsHelpText);

			if (haveOptions)
			{
				_writer.WriteLine("<options> available:");
				selectedCommand.Options.WriteOptionDescriptions(_writer);
			}
			_writer.WriteLine();
		}

		public void ShowParsedCommand(ConsoleCommand consoleCommand)
		{
			if (!consoleCommand.TraceCommandAfterParse)
			{
				return;
			}

			string[] skippedProperties = new[]
			{
				"Command",
				"OneLineDescription",
				"Options",
				"TraceCommandAfterParse",
				"RemainingArgumentsCount",
				"RemainingArgumentsHelpText",
				"RequiredOptions"
			};

			var properties = consoleCommand.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(p => !skippedProperties.Contains(p.Name));

			var fields = consoleCommand.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
				.Where(p => !skippedProperties.Contains(p.Name));

			var allValuesToTrace = new Dictionary<string, string>();

			foreach (var property in properties)
			{
				object value = property.GetValue(consoleCommand, new object[0]);
				allValuesToTrace[property.Name] = value != null ? value.ToString() : "null";
			}

			foreach (var field in fields)
			{
				allValuesToTrace[field.Name] = MakeObjectReadable(field.GetValue(consoleCommand));
			}

			_writer.WriteLine();

			var introLine = string.Format("Executing {0}", consoleCommand.Command);

			if (string.IsNullOrEmpty(consoleCommand.OneLineDescription))
			{
				introLine = introLine + ":";
			}
			else
			{
				introLine = introLine + " (" + consoleCommand.OneLineDescription + "):";
			}

			_writer.WriteLine(introLine);

			foreach (var value in allValuesToTrace.OrderBy(k => k.Key))
			{
				_writer.WriteLine("    {0}:{1}", value.Key, value.Value);
			}

			_writer.WriteLine();
		}

		private static string MakeObjectReadable(object value)
		{
			string readable;

			if (value is System.Collections.IEnumerable && !(value is string))
			{
				readable = "";
				var separator = "";

				foreach (var member in (IEnumerable)value)
				{
					readable += separator + MakeObjectReadable(member);
					separator = ", ";
				}
			}
			else if (value != null)
			{
				readable = value.ToString();
			}
			else
			{
				readable = "null";
			}
			return readable;
		}
	}
}
