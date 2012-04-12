using System;

namespace AppHarbor.Commands
{
	public class AddHostnameCommand : ICommand
	{
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly IAppHarborClient _appharborClient;

		public AddHostnameCommand(IApplicationConfiguration applicationConfiguration, IAppHarborClient appharborClient)
		{
			_applicationConfiguration = applicationConfiguration;
			_appharborClient = appharborClient;
		}

		public void Execute(string[] arguments)
		{
			if (arguments.Length == 0)
			{
				throw new CommandException("No hostname was specified");
			}
			throw new NotImplementedException();
		}
	}
}
