using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Tar;
using System;

namespace AppHarbor
{
	public static class CompressionExtensions
	{
		public static void ToTar(this DirectoryInfo sourceDirectory, Stream output, string[] excludedDirectoryNames)
		{
			string previousCurrDir = Environment.CurrentDirectory;
			try
			{
				// Chgange the current directory to the source directory
				// because the Tar library use it as a reference point
				// and it will leave the specified source-directory in each
				// tar file path otherwise.
				Environment.CurrentDirectory = sourceDirectory.FullName;

				TarDirectory(sourceDirectory, output, excludedDirectoryNames);
			}
			finally
			{
				Environment.CurrentDirectory = previousCurrDir;
			}
		}

		private static void TarDirectory(DirectoryInfo sourceDirectory, Stream output, string[] excludedDirectoryNames)
		{
			var archive = TarArchive.CreateOutputTarArchive(output);

			archive.RootPath = sourceDirectory.FullName.Replace(Path.DirectorySeparatorChar, '/').TrimEnd('/');

			var entries = GetFiles(sourceDirectory, excludedDirectoryNames)
				.Select(x => TarEntry.CreateEntryFromFile(x.FullName))
				.ToList();

			var entriesCount = entries.Count();

			var progressBar = new MegaByteProgressBar();
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
