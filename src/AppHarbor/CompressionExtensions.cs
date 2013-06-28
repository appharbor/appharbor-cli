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

			ProgressBarPresenter progressBar = ProgressBarFactory.CreateMegaByteProgressBar();
			for (var i = 0; i < entriesCount; i++)
			{
				archive.WriteEntry(entries[i], true);

				progressBar.Update("Packing files", entries.Take(i + 1).Sum(x => x.Size), entries.Sum(x => x.Size));
			}

			archive.Close();
		}

		private static IEnumerable<FileInfo> GetFiles(DirectoryInfo directory, string[] excludedDirectories)
		{
			return directory.GetFiles("*", SearchOption.TopDirectoryOnly)
				.Concat(directory.GetDirectories()
				.Where(x => !excludedDirectories.Select(y => y.ToLower()).Contains(x.Name.ToLower()))
				.SelectMany(x => GetFiles(x, excludedDirectories)));
		}
	}
}
