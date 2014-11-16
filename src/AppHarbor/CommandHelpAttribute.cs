using System;

namespace AppHarbor
{
	public class CommandHelpAttribute : Attribute
	{
		public CommandHelpAttribute(string description, string options = "", string alias = "")
		{
			Description = description;
			Options = options;
			Alias = alias;
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

		public string Alias
		{
			get;
			private set;
		}
	}
}
