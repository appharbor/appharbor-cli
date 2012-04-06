using System.Collections.Generic;

namespace AppHarbor
{
	public interface IGitExecutor
	{
		IList<string> Execute(string command);
	}
}
