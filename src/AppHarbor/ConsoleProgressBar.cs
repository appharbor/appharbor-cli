using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.S3.Transfer;

namespace AppHarbor
{
	public class ConsoleProgressBar
	{
		private const char ProgressBarCharacter = '\u2592';

		private KeyValuePair<DateTime, double> _lastProgressEvent;
		private readonly IList<double> _perSecondAverages;

		public ConsoleProgressBar()
		{
			_perSecondAverages = new List<double>();
		}

		public void Update(string message, string itemType, long processedItems, long totalItems)
		{
			var secondsSinceLastAverage = (DateTime.Now - _lastProgressEvent.Key).TotalSeconds;

			if (secondsSinceLastAverage > 2)
			{
				if (_lastProgressEvent.Key != DateTime.MinValue)
				{
					var itemsSinceLastProgress = processedItems - _lastProgressEvent.Value;

					var itemsPerSecond = itemsSinceLastProgress / secondsSinceLastAverage;
					_perSecondAverages.Add(itemsPerSecond);
					if (_perSecondAverages.Count() > 20)
					{
						_perSecondAverages.RemoveAt(0);
					}
				}
				_lastProgressEvent = new KeyValuePair<DateTime, double>(DateTime.Now, processedItems);
			}

			var itemsRemaining = totalItems - processedItems;
			var timeEstimate = _perSecondAverages.Count() < 1 ? "Estimating time left" :
				string.Format("{0} left", TimeSpan
				.FromSeconds(itemsRemaining / WeightedAverage(_perSecondAverages))
				.GetHumanized());

			var percentDone = (processedItems * 100) / totalItems;
			ConsoleProgressBar.Render(percentDone, string.Format("{0} ({1}% of {2:0.0} {3}). {4}",
				message, percentDone, totalItems, itemType, timeEstimate));
		}

		private static void Render(double percentage, string message)
		{
			Console.CursorLeft = 0;

			using (new ForegroundColor(ConsoleColor.Cyan))
			{
				try
				{
					Console.CursorVisible = false;

					int width = Console.WindowWidth - 1;
					int newWidth = (int)((width * percentage) / 100d);
					string progressBar = string.Empty
						.PadRight(newWidth, ProgressBarCharacter)
						.PadRight(width - newWidth, ' ');

					Console.Write(progressBar);
					message = message ?? string.Empty;

					Console.WriteLine();

					OverwriteConsoleMessage(message);
					Console.CursorTop--;
				}
				finally
				{
					Console.CursorVisible = true;
				}
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

		private static double WeightedAverage(IList<double> input, int spread = 40)
		{
			if (input.Count == 1)
			{
				return input.Average();
			}

			var weightdifference = spread / (input.Count() - 1);
			var averageWeight = 50;
			var startWeight = averageWeight - spread / 2;

			return input.Select((x, i) => x * (startWeight + (i * weightdifference)))
				.Sum() / (averageWeight * input.Count());
		}
	}
}
