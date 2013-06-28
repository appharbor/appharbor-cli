using System;
using System.Diagnostics;

namespace AppHarbor
{
	/// <summary>
	/// Factory for creating ProgressBar that match the current environment
	/// in which the application runs.
	/// </summary>
	public static class ProgressBarFactory
	{
		private static Nullable<bool> hasConsoleWindow;
		public static ProgressBarPresenter CreateMegaByteProgressBar()
		{
			if (HasConsoleWindow)
			{
				return new MegaByteProgressBar();
			}
			else
			{
				return new NullProgressBar();
			}
		}

		private static bool HasConsoleWindow
		{
			get
			{
				if (hasConsoleWindow == null)
				{
					try
					{
						int w = Console.BufferWidth;
						hasConsoleWindow = true;
					}
					catch (Exception)
					{
						hasConsoleWindow = false;
					}
				}
				return hasConsoleWindow.Value;
			}
		}
	}
}
