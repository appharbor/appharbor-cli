using System;

namespace AppHarbor
{
	/// <summary>
	/// Helper class for console window information.
	/// </summary>
	static class ConsoleWindowHelper
	{
		/// <summary>
		/// Get indication if a console window is available or we run
		/// without a window (in CI environment).
		/// </summary>
		public static bool HasConsoleWindow
		{
			get
			{
				bool hasConsoleWindow;
				try
				{
					int w = Console.BufferWidth;
					hasConsoleWindow = true;
				}
				catch (Exception)
				{
					hasConsoleWindow = false;
				}
				return hasConsoleWindow;
			}
		}
	}
}
