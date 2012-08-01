using System.Collections.Generic;
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

			var entries = GetFiles(sourceDirectory).Select(x => TarEntry.CreateEntryFromFile(x.FullName));

			foreach (var entry in entries)
			{
				archive.WriteEntry(entry, true);
			}

			archive.Close();
		}

		private static FileInfo[] GetFiles(DirectoryInfo directory)
		{
			var files = directory.GetFiles("*", SearchOption.TopDirectoryOnly);
			foreach (var nestedDirectory in directory.GetDirectories())
			{
				files.Concat(GetFiles(nestedDirectory));
			}

			return files;
		}
	}
}
