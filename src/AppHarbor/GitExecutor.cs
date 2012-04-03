using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AppHarbor
{
	public class GitExecutor : IGitExecutor
	{
		private readonly FileInfo _gitExecutable;

		public GitExecutor()
		{
			var programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
			var gitExecutablePath = Path.Combine(programFilesPath, "Git", "bin", "git.exe");
			_gitExecutable = new FileInfo(gitExecutablePath);
		}

		public virtual void Execute(string command, DirectoryInfo repositoryDirectory)
		{
			var processArguments = new StringBuilder();

			processArguments.AppendFormat("/C ");
			processArguments.AppendFormat("{0} ", _gitExecutable.Name);
			processArguments.AppendFormat("{0} ", command);
			processArguments.AppendFormat("--git-dir=\"{0}\" ", repositoryDirectory.FullName);

			var process = new Process
			{
				StartInfo = new ProcessStartInfo("cmd.exe")
				{
					Arguments = processArguments.ToString(),
					CreateNoWindow = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					WorkingDirectory = _gitExecutable.Directory.FullName,
				},
			};

			using (process)
			{
				process.Start();
				process.WaitForExit();

				string error = process.StandardError.ReadToEnd();
				if (!string.IsNullOrEmpty(error))
				{
					throw new InvalidOperationException(error);
				}
			}
		}

		public virtual bool IsInstalled()
		{
			try
			{
				Execute("--version", new DirectoryInfo("c:\\"));
			}
			catch (InvalidOperationException)
			{
				return false;
			}
			return true;
		}
	}
}
