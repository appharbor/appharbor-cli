using System;
using System.Linq;
using System.Collections.Generic;

namespace AppHarbor
{
	public class AliasMatcher : IAliasMatcher
	{
		private readonly IEnumerable<Type> _candidateTypes;

		public AliasMatcher(IEnumerable<Type> candidateTypes)
		{
			if (candidateTypes.Any(x => !x.GetCustomAttributes(true).OfType<CommandHelpAttribute>().Any()))
			{
				throw new ArgumentException("All candidate types must be decorated with CommandHelpAttribute");
			}
			_candidateTypes = candidateTypes;
		}

		public virtual Type GetMatchedType(string commandArgument)
		{
			try
			{
				return _candidateTypes.Single(
					x => x.GetCustomAttributes(true).OfType<CommandHelpAttribute>().Single().Alias.ToLower() == commandArgument.ToLower());
			}
			catch (InvalidOperationException)
			{
				throw new ArgumentException("Command doesn't match any command alias");
			}
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
