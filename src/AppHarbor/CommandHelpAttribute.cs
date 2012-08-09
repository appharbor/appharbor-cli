using System;

namespace AppHarbor
{
	public class CommandHelpAttribute : Attribute
	{
		public CommandHelpAttribute(string description, string additionalArgumentsHelp = "", int numberOfAdditionalArguments = 0, string alias = "")
		{
			Alias = alias;
			Description = description;
			AdditionalArgumentHelpText = additionalArgumentsHelp;
			AdditionalArgumentsCount = numberOfAdditionalArguments;
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

		public string AdditionalArgumentHelpText
		{
			get;
			private set;
		}

		public int AdditionalArgumentsCount
		{
			get;
			private set;
		}
	}
}
