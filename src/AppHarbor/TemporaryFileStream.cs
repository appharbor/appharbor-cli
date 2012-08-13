using System.IO;

namespace AppHarbor
{
	public class TemporaryFileStream : FileStream
	{
		public TemporaryFileStream()
			: base(Path.GetTempFileName(), FileMode.Create, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.DeleteOnClose)
		{
		}
	}
}
