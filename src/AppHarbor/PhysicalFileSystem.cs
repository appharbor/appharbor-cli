using System.IO;

namespace AppHarbor
{
	public class PhysicalFileSystem : IFileSystem
	{
		public void Delete(string path)
		{
			var directory = new DirectoryInfo(path);
			if (directory.Exists)
			{
				directory.Delete(recursive: true);
				return;
			}

			var file = new FileInfo(path);
			if (file.Exists)
			{
				file.Delete();
			}
		}

		public Stream OpenRead(string path)
		{
			var file = new FileInfo(path);
			if (file.Exists)
			{
				return file.OpenRead();
			}

			throw new FileNotFoundException();
		}

		public Stream OpenWrite(string path)
		{
			var file = new FileInfo(path);
			if (!file.Directory.Exists)
			{
				file.Directory.Create();
			}

			return file.OpenWrite();
		}
	}
}
