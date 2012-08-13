using NDesk.Options;

namespace AppHarbor
{
	public abstract class Command
	{
		private readonly OptionSet _optionSet;

		public Command()
		{
			_optionSet = new OptionSet();
		}

		public abstract void Execute(string[] arguments);

		public OptionSet OptionSet
		{
			get
			{
				return _optionSet;
			}
		}
	}
}
