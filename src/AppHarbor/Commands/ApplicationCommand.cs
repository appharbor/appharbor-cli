namespace AppHarbor.Commands
{
	public abstract class ApplicationCommand : Command
	{
		private readonly IApplicationConfiguration _applicationConfiguration;
		private string _applicationId;

		public ApplicationCommand(IApplicationConfiguration applicationConfiguration)
		{
			_applicationConfiguration = applicationConfiguration;
			OptionSet.Add("a|application=", "Optional application id", x => _applicationId = x);
		}

		protected string ApplicationId
		{
			get
			{
				return _applicationId ?? _applicationConfiguration.GetApplicationId();
			}
		}
	}
}
