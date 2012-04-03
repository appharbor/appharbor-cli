using System;
using System.IO;

namespace AppHarbor
{
	public class PhysicalFileSystem : IFileSystem
	{
		public void Delete(string path)
		{
			throw new NotImplementedException();
		}

		public Stream OpenRead(string path)
		{
			throw new NotImplementedException();
		}

		public Stream OpenWrite(string path)
		{
			throw new NotImplementedException();
		}
	}
}
