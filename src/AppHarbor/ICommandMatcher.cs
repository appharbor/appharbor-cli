using System;

namespace AppHarbor
{
	public interface ICommandMatcher
	{
		Type GetMatchedType(string commandArgument);
		bool IsSatisfiedBy(string commandArgument);
	}
}
