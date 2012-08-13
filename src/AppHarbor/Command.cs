using System.IO;
using System.Linq;
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

		public virtual void Execute(string[] arguments)
		{
			var commandArguments = OptionSet.Parse(arguments).ToArray();
			InnerExecute(commandArguments);
		}

		protected abstract void InnerExecute(string[] arguments);

		public OptionSet OptionSet
		{
			get
			{
				return _optionSet;
			}
		}

		public void WriteUsage(string invokedWith, TextWriter writer)
		{
			var commandHelpAttribute = this.GetType().GetCustomAttributes(true).OfType<CommandHelpAttribute>().Single();
			writer.WriteLine("Command description: {0}", commandHelpAttribute.Description);
			writer.WriteLine();

			writer.WriteLine("Expected usage: appharbor {0} {1} [COMMAND-OPTIONS]",
				invokedWith,
				commandHelpAttribute.Options);

			writer.WriteLine("Available options:");
			OptionSet.WriteOptionDescriptions(writer);
		}
	}
}
