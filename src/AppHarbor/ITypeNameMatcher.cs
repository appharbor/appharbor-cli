using System;

namespace AppHarbor
{
	public interface ITypeNameMatcher
	{
		Type GetMatchedType(string commandArgument);
	}
}
