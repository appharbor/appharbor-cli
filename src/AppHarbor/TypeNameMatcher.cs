using System;
using System.Collections.Generic;

namespace AppHarbor
{
	public class TypeNameMatcher
	{
		private readonly IEnumerable<Type> _candidateTypes;

		public TypeNameMatcher(IEnumerable<Type> candidateTypes)
		{
			_candidateTypes = candidateTypes;
		}
	}
}
