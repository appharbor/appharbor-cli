using System;

namespace AppHarbor.Commands
{
	public class AddConfigCommand : ICommand
	{
		private readonly IApplicationConfiguration _applicationConfiguration;

		public AddConfigCommand(IApplicationConfiguration applicationConfiguration)
		{
			_applicationConfiguration = applicationConfiguration;
		}

		public void Execute(string[] arguments)
		{
			throw new NotImplementedException();
		}
	}
}
