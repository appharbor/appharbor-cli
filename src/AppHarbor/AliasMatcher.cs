using System;
using System.Collections.Generic;

namespace AppHarbor
{
	public class AliasMatcher : IAliasMatcher
	{
		private readonly IEnumerable<Type> _candidateTypes;

		public AliasMatcher(IEnumerable<Type> candidateTypes)
		{
			_candidateTypes = candidateTypes;
		}

		public Type GetMatchedType(string commandArgument)
		{
			throw new NotImplementedException();
		}
	}
}
