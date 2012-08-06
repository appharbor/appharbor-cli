using System.IO;

namespace AppHarbor
{
	public class ConsoleProgressBar
	{
		private readonly TextWriter _writer;

		public ConsoleProgressBar(TextWriter writer)
		{
			_writer = writer;
		}
	}
}
