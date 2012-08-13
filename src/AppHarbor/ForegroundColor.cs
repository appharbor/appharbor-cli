using System;

namespace AppHarbor
{
	public class ForegroundColor : IDisposable
	{
		private readonly ConsoleColor _originalColor;

		public ForegroundColor(ConsoleColor color)
		{
			_originalColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
		}

		public void Dispose()
		{
			Console.ForegroundColor = _originalColor;
		}
	}
}
