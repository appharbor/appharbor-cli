using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Tar;

namespace AppHarbor
{
	public static class CompressionExtensions
	{
		public static void ToTar(this DirectoryInfo sourceDirectory, Stream output, string[] excludedDirectoryNames)
		{
			var archive = TarArchive.CreateOutputTarArchive(output);

			archive.RootPath = sourceDirectory.FullName.Replace(Path.DirectorySeparatorChar, '/').TrimEnd('/');

			var entries = GetFiles(sourceDirectory, excludedDirectoryNames)
				.Select(x => TarEntry.CreateEntryFromFile(x.FullName))
				.ToList();

			var entriesCount = entries.Count();

			var consoleProgressBar = new ConsoleProgressBar();
			for (var i = 0; i < entriesCount; i++)
			{
				archive.WriteEntry(entries[i], true);

				consoleProgressBar.Update("Packing files", "files", i, entriesCount);
			}

			archive.Close();
		}

		private static IEnumerable<FileInfo> GetFiles(DirectoryInfo directory, string[] excludedDirectories)
		{
			return directory.GetFiles("*", SearchOption.TopDirectoryOnly)
				.Concat(directory.GetDirectories()
				.Where(x => !excludedDirectories.Contains(x.Name))
				.SelectMany(x => GetFiles(x, excludedDirectories)));
		}
	}
}
