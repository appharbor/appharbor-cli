using System;

namespace AppHarbor
{
	public class CommandHelpAttribute : Attribute
	{
		public CommandHelpAttribute(string description, string options = "", string alias = "")
		{
			Alias = alias;
			Description = description;
			Options = options;
		}

		public string Alias
		{
			get;
			private set;
		}

		public string Description
		{
			get;
			private set;
		}

		public string Options
		{
			get;
			private set;
		}
	}
}
