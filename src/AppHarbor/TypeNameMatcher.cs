using System;
using System.Collections.Generic;
using System.Linq;

namespace AppHarbor
{
	public class TypeNameMatcher<T> : ITypeNameMatcher
	{
		private readonly IEnumerable<Type> _candidateTypes;

		public TypeNameMatcher(IEnumerable<Type> candidateTypes)
		{
			if (candidateTypes.Any(x => !typeof(T).IsAssignableFrom(x)))
			{
				throw new ArgumentException(string.Format("{0} must be assignable from all injected types", typeof(T).FullName), "candidateTypes");
			}
			_candidateTypes = candidateTypes;
		}

		public virtual Type GetMatchedType(string command)
		{
			try
			{
				return _candidateTypes.Single(x => x.Name.ToLower() == string.Concat(command, "Command").ToLower());
			}
			catch (InvalidOperationException)
			{
			}

			throw new ArgumentException(string.Format("The command \"{0}\" is invalid.", command));
		}


		public bool IsSatisfiedBy(string commandArgument)
		{
			try
			{
				GetMatchedType(commandArgument);
				return true;
			}
			catch (ArgumentException)
			{
			}

			return false;
		}
	}
}
