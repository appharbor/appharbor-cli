using System.Collections.Generic;
using System.IO;

namespace AppHarbor
{
	public interface IGitExecutor
	{
		IList<string> Execute(string command, DirectoryInfo repositoryDirectory);
	}
}
