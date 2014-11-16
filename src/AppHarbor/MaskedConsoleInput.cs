using System;

namespace AppHarbor
{
	public class MaskedConsoleInput : IMaskedInput
	{
		public virtual string Get()
		{
			var input = string.Empty;
			ConsoleKeyInfo consoleKey;

			do
			{
				consoleKey = Console.ReadKey(true);

				if (consoleKey.Key != ConsoleKey.Backspace && consoleKey.Key != ConsoleKey.Enter)
				{
					input += consoleKey.KeyChar;
					Console.Write("*");
				}
				else if (consoleKey.Key == ConsoleKey.Backspace && input.Length > 0)
				{
					input = input.Substring(0, (input.Length - 1));
					Console.Write("\b \b");
				}
			}
			while (consoleKey.Key != ConsoleKey.Enter);

			return input;
		}
	}
}
