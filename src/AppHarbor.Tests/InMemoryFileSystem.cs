using System.Collections.Generic;
using System.IO;

namespace AppHarbor.Tests
{
	public class InMemoryFileSystem : IFileSystem
	{
		private readonly IDictionary<string, byte[]> _files;

		public InMemoryFileSystem()
		{
			_files = new Dictionary<string, byte[]>();
		}

		public void Delete(string path)
		{
			_files.Remove(path);
		}

		public Stream OpenRead(string path)
		{
			byte[] bytes;
			if (!_files.TryGetValue(path, out bytes))
			{
				throw new FileNotFoundException();
			}

			return new MemoryStream(bytes);
		}

		public Stream OpenWrite(string path)
		{
			return new DelegateOutputStream(x =>
			{
				var bytes = x.ToArray();
				_files.Add(path, bytes);
			});
		}

		public IDictionary<string, byte[]> Files
		{
			get
			{
				return _files;
			}
		}
	}
}
