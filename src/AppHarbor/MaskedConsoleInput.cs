using System;

namespace AppHarbor
{
	public class MaskedConsoleInput : IMaskedInput
	{
		public string Get()
		{
			string input = "";
			ConsoleKeyInfo key;

			do
			{
				key = Console.ReadKey(true);

				if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
				{
					input += key.KeyChar;
					Console.Write("*");
				}
				else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
				{
					input = input.Substring(0, (input.Length - 1));
					Console.Write("\b \b");
				}
			}
			while (key.Key != ConsoleKey.Enter);

			return input;
		}
	}
}
