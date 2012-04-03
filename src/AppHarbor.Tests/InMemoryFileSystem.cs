using System;
using System.IO;

namespace AppHarbor.Tests
{
	public class InMemoryFileSystem : IFileSystem
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
