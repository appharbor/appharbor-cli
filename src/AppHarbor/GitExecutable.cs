using System;
using System.IO;

namespace AppHarbor
{
	public class GitExecutable
	{
		private readonly FileInfo _gitExecutable;

		public GitExecutable()
		{
			var programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
			var gitExecutablePath = Path.Combine(programFilesPath, "Git", "bin", "git.exe");
			_gitExecutable = new FileInfo(gitExecutablePath);
		}
	}
}
