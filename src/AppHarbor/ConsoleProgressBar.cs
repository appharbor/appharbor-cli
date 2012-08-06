using System;
using System.IO;

namespace AppHarbor
{
	public class ConsoleProgressBar
	{
		private const char ProgressBarCharacter = '\u2592';

		public static void Render(double percentage, ConsoleColor color, string message)
		{
			ConsoleColor originalColor = Console.ForegroundColor;
			Console.CursorLeft = 0;

			try
			{
				Console.CursorVisible = false;
				Console.ForegroundColor = color;

				int width = Console.WindowWidth - 1;
				int newWidth = (int)((width * percentage) / 100d);
				string progressBar = string.Empty
					.PadRight(newWidth, ProgressBarCharacter)
					.PadRight(width - newWidth, ' ');

				Console.Write(progressBar);
				message = message ?? string.Empty;

				Console.CursorTop++;

				OverwriteConsoleMessage(message);
				Console.CursorTop--;
			}
			finally
			{
				Console.ForegroundColor = originalColor;
				Console.CursorVisible = true;
			}
		}

		private static void OverwriteConsoleMessage(string message)
		{
			Console.CursorLeft = 0;
			int maxCharacterWidth = Console.WindowWidth - 1;
			if (message.Length > maxCharacterWidth)
			{
				message = message.Substring(0, maxCharacterWidth - 3) + "...";
			}
			message = message + new string(' ', maxCharacterWidth - message.Length);
			Console.Write(message);
		}
	}
}
