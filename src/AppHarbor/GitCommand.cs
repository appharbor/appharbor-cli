using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AppHarbor
{
	public class GitCommand : IGitCommand
	{
		private readonly FileInfo _gitExecutable;

		public GitCommand()
		{
			var programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
			var gitExecutablePath = Path.Combine(programFilesPath, "Git", "bin", "git.exe");
			_gitExecutable = new FileInfo(gitExecutablePath);
		}

		public virtual IList<string> Execute(string command)
		{
			var processArguments = new StringBuilder();

			processArguments.AppendFormat("/C ");
			processArguments.AppendFormat("\"{0}\" ", _gitExecutable.FullName);
			processArguments.AppendFormat("{0} ", command);

			var process = new Process
			{
				StartInfo = new ProcessStartInfo("cmd.exe")
				{
					Arguments = processArguments.ToString(),
					CreateNoWindow = true,
					RedirectStandardError = true,
					RedirectStandardOutput = true,
					UseShellExecute = false,
					WorkingDirectory = CurrentDirectory.FullName,
				},
			};

			using (process)
			{
				process.Start();
				process.WaitForExit();
				string error = process.StandardError.ReadToEnd();
				if (!string.IsNullOrEmpty(error))
				{
					throw new GitCommandException(error);
				}

				var output = new List<string>();
				while (process.StandardOutput.Peek() > 0)
				{
					output.Add(process.StandardOutput.ReadLine());
				}

				return output;
			}
		}

		private static DirectoryInfo CurrentDirectory
		{
			get
			{
				return new DirectoryInfo(Directory.GetCurrentDirectory());
			}
		}
	}
}
