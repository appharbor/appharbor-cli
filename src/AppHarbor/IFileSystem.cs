using System.IO;

namespace AppHarbor
{
	public interface IFileSystem
	{
		void Delete(string path);
		Stream OpenRead(string path);
		Stream OpenWrite(string path);
	}
}
