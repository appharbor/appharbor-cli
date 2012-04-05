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

		public virtual Type GetMatchedType(string commandArgument)
		{
			var splitted = commandArgument.Split(':');
			var commandName = splitted.Last();
			var scope = splitted.Reverse().Skip(1).FirstOrDefault();

			var typeNameSuffix = "Command";
			var scopedTypes = _candidateTypes
				.Where(x => x.Name.ToLower().EndsWith(string.Concat(scope, typeNameSuffix).ToLower()));

			Type command;

			try
			{
				return scopedTypes.Single(x => x.Name.ToLower() == string.Concat(commandName, typeNameSuffix).ToLower());
			}
			catch (InvalidOperationException)
			{
			}

			try
			{
				command = scopedTypes.SingleOrDefault(x => x.Name.ToLower().StartsWith(commandName.ToLower()));
				if (command != null)
				{
					return command;
				}
			}
			catch (InvalidOperationException)
			{
				throw new ArgumentException(string.Format("More than one command matches \"{0}\".", commandName));
			}

			throw new ArgumentException(string.Format("No commands matches \"{0}\".", commandName));
		}
	}
}
