namespace AppHarbor.Commands
{
	public abstract class ApplicationCommand : Command
	{
		private readonly IApplicationConfiguration _applicationConfiguration;

		public ApplicationCommand(IApplicationConfiguration applicationConfiguration)
		{
			_applicationConfiguration = applicationConfiguration;
		}

		protected string ApplicationId
		{
			get
			{
				return _applicationConfiguration.GetApplicationId();
			}
		}
	}
}
