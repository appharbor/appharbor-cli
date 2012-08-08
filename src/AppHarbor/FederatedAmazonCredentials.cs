using Amazon.Runtime;

namespace AppHarbor
{
	public class FederatedAmazonCredentials
	{
		public string AccessKeyId { get; set; }
		public string SecretAccessKey { get; set; }
		public string SessionToken { get; set; }

		public SessionAWSCredentials GetSessionCredentials()
		{
			return new SessionAWSCredentials(AccessKeyId, SecretAccessKey, SessionToken);
		}
	}
}
