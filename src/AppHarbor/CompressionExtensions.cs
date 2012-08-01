using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Tar;

namespace AppHarbor
{
	public static class CompressionExtensions
	{
		public static void ToTar(this DirectoryInfo sourceDirectory, Stream output)
		{
			var archive = TarArchive.CreateOutputTarArchive(output);

			archive.RootPath = sourceDirectory.FullName.Replace(Path.DirectorySeparatorChar, '/').TrimEnd('/');

			var entries =
				from x in sourceDirectory.GetFiles("*", SearchOption.AllDirectories)
				select TarEntry.CreateEntryFromFile(x.FullName);

			foreach (var entry in entries)
			{
				archive.WriteEntry(entry, true);
			}

			archive.Close();
		}
	}
}
