using Amazon.Runtime;

namespace AppHarbor
{
	public class FederatedUploadCredentials
	{
		public string AccessKeyId { get; set; }
		public string SecretAccessKey { get; set; }
		public string SessionToken { get; set; }
		public string Bucket { get; set; }
		public string ObjectKey { get; set; }

		public SessionAWSCredentials GetSessionCredentials()
		{
			return new SessionAWSCredentials(AccessKeyId, SecretAccessKey, SessionToken);
		}
	}
}
