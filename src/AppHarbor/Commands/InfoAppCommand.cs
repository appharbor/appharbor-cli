using System.Collections.Generic;
using System.IO;
using NDesk.Options;

namespace AppHarbor.Commands
{
	[CommandHelp("Get application details")]
	public class InfoAppCommand : ConsoleCommand
	{
		private readonly IAppHarborClient _client;
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly TextWriter _writer;

		public InfoAppCommand(IAppHarborClient client, IApplicationConfiguration applicationConfiguration, TextWriter writer)
		{
			_client = client;
			_applicationConfiguration = applicationConfiguration;
			_writer = writer;

			Options = new OptionSet()
            {
                {"b|booleanOption", "Boolean flag option", b => BooleanOption = true},
                {"l|list=", "Values to add to list", v => OptionalArgumentList.Add(v)},
                {"r|requiredArguments=", "Optional string argument requiring a value be specified afterwards", s => OptionalArgument1 = s},
                {"o|optionalArgument:", "Optional String argument which is null if no value follow is specified", s => OptionalArgument2 = s ?? "<no argument specified>"}
            };

			this.HasRequiredOption("requiredOption=", "Required string argument also requiring a value.", s => { });
			this.HasOption("anotherOptional=", "Another way to specify optional arguments", s => { });

			HasAdditionalArguments(2, "<Argument1> <Argument2>");
		}


		public string Argument1;
		public string Argument2;
		public string OptionalArgument1;
		public string OptionalArgument2;
		public bool BooleanOption;
		public List<string> OptionalArgumentList = new List<string>();

		public override void Run(string[] arguments)
		{
			var id = _applicationConfiguration.GetApplicationId();
			var application = _client.GetApplication(id);

			_writer.WriteLine("Id: {0}", application.Slug);
			_writer.WriteLine("Name: {0}", application.Name);
			_writer.WriteLine("Region: {0}", application.RegionIdentifier);
		}
	}
}
