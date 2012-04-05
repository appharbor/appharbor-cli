using System;

namespace AppHarbor
{
	public interface ITypeNameMatcher
	{
		Type GetMatchedType(string commandName, string scope);
	}
}
