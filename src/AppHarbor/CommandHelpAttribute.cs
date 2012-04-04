using System;

namespace AppHarbor
{
	public class CommandHelpAttribute : Attribute
	{
		public CommandHelpAttribute(string description, string options = "")
		{
			Description = description;
			Options = options;
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
