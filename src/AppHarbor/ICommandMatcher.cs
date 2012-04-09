using System;

namespace AppHarbor
{
	public interface ICommandMatcher
	{
		Type GetMatchedType(string commandArgument);
	}
}
