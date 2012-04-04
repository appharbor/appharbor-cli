using System;
using System.Collections.Generic;

namespace AppHarbor
{
	public interface IGitExecutor
	{
		IEnumerable<string> Execute(string command, System.IO.DirectoryInfo repositoryDirectory);
	}
}
