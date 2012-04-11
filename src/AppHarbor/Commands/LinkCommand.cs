using System;

namespace AppHarbor.Commands
{
	public class LinkCommand : ICommand
	{
		private readonly IApplicationConfiguration _applicationConfiguration;

		public LinkCommand(IApplicationConfiguration applicationConfiguration)
		{
			_applicationConfiguration = applicationConfiguration;
		}

		public void Execute(string[] arguments)
		{
			throw new NotImplementedException();
		}
	}
}
