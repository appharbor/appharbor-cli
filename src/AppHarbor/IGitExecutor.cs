using System;

namespace AppHarbor
{
	public interface IGitExecutor
	{
		void Execute(string command, System.IO.DirectoryInfo repositoryDirectory);
	}
}
