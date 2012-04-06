using System.Collections.Generic;

namespace AppHarbor
{
	public interface IGitCommand
	{
		IList<string> Execute(string command);
	}
}
