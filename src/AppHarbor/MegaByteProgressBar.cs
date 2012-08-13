namespace AppHarbor
{
	public class MegaByteProgressBar : ConsoleProgressBar
	{
		public MegaByteProgressBar()
			: base("MB", x => x / (double)1048576)
		{
		}
	}
}
