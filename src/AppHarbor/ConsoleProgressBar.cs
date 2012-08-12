using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.S3.Transfer;

namespace AppHarbor
{
	public class ConsoleProgressBar
	{
		private const char ProgressBarCharacter = '\u2592';
		private KeyValuePair<DateTime, double> _lastUploadProgressEvent;
		private IList<double> _bytesPerSecondAverages = new List<double>();

		public static void Render(double percentage, string message)
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

		public void RenderProgress(object sender, UploadProgressArgs uploadProgressArgs)
		{
			var secondsSinceLastAverage = (DateTime.Now - _lastUploadProgressEvent.Key).TotalSeconds;

			if (secondsSinceLastAverage > 2)
			{
				if (_lastUploadProgressEvent.Key != DateTime.MinValue)
				{
					var bytesSinceLastProgress = uploadProgressArgs.TransferredBytes - _lastUploadProgressEvent.Value;

					var bytesPerSecond = bytesSinceLastProgress / secondsSinceLastAverage;
					_bytesPerSecondAverages.Add(bytesPerSecond);
					if (_bytesPerSecondAverages.Count() > 20)
					{
						_bytesPerSecondAverages.RemoveAt(0);
					}
				}
				_lastUploadProgressEvent = new KeyValuePair<DateTime, double>(DateTime.Now, uploadProgressArgs.TransferredBytes);
			}

			var bytesRemaining = uploadProgressArgs.TotalBytes - uploadProgressArgs.TransferredBytes;
			var timeEstimate = _bytesPerSecondAverages.Count() < 1 ? "Estimating time left" :
				string.Format("{0} left", TimeSpan
				.FromSeconds(bytesRemaining / WeightedAverage(_bytesPerSecondAverages))
				.GetHumanized());

			ConsoleProgressBar.Render(uploadProgressArgs.PercentDone,
				string.Format("Uploading package ({0}% of {1:0.0} MB). {2}",
					uploadProgressArgs.PercentDone, uploadProgressArgs.TotalBytes / 1048576, timeEstimate));
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
