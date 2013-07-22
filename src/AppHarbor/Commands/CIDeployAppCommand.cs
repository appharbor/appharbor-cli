using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using Amazon.S3;
using Amazon.S3.Transfer;
using RestSharp;

namespace AppHarbor.Commands
{
	[CommandHelp("Deploy application in CI environment", alias: "ci-deploy")]
	public class CIDeployAppCommand : ApplicationCommand
	{
		private string _message;
		private DirectoryInfo _sourceDirectory;
		private string _username;
		private string _password;

		private readonly IRestClient _restClient;
		private readonly TextWriter _writer;
		private readonly IProgressBar _progressBar;

		private readonly IList<string> _excludedDirectories;

		public CIDeployAppCommand(IApplicationConfiguration applicationConfiguration, TextWriter writer, IProgressBar progressBar)
			: base(applicationConfiguration)
		{
			_restClient = new RestClient("https://packageclient.apphb.com/");
			_writer = writer;
			_progressBar = progressBar;

			_sourceDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
			OptionSet.Add("source-directory=", "Set source directory", x => _sourceDirectory = new DirectoryInfo(x));

			_excludedDirectories = new List<string> { ".git", ".hg" };
			OptionSet.Add("e|excluded-directory=", "Add excluded directory name", x => _excludedDirectories.Add(x));

			OptionSet.Add("m|message=", "Specify commit message", x => _message = x);

			OptionSet.Add("u|user=", "Optional. Specify the user to use", x => _username = x);
			OptionSet.Add("p|password=", "Optional. Specify the password of the user", x => _password = x);
		}

		protected override void InnerExecute(string[] arguments)
		{
			_writer.WriteLine("Ensure login credentials...");
			string accessToken = GetAccessToken();
			_writer.WriteLine();

			_writer.WriteLine("Getting upload credentials... ");
			_writer.WriteLine();

			var uploadCredentials = GetCredentials();

			var temporaryFileName = Path.GetTempFileName();
			try
			{
				using (var packageStream = new FileStream(temporaryFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
				using (var gzipStream = new GZipStream(packageStream, CompressionMode.Compress, true))
				{
					_sourceDirectory.ToTar(gzipStream, excludedDirectoryNames: _excludedDirectories.ToArray(), progressBar: _progressBar);
				}

				using (var s3Client = new AmazonS3Client(uploadCredentials.GetSessionCredentials()))
				using (var transferUtility = new TransferUtility(s3Client))
				{
					var request = new TransferUtilityUploadRequest
					{
						FilePath = temporaryFileName,
						BucketName = uploadCredentials.Bucket,
						Key = uploadCredentials.ObjectKey,
						Timeout = (int)TimeSpan.FromHours(2).TotalMilliseconds,
					};

					request.UploadProgressEvent += (object x, UploadProgressArgs y) => _progressBar
						.Update("Uploading package", y.TransferredBytes, y.TotalBytes);

					transferUtility.Upload(request);

					Console.CursorTop++;
					_writer.WriteLine();
				}
			}
			finally
			{
				File.Delete(temporaryFileName);
			}

			TriggerAppHarborBuild(uploadCredentials, accessToken);
		}

		private FederatedUploadCredentials GetCredentials()
		{
			var urlRequest = new RestRequest("applications/{slug}/uploadCredentials", Method.POST);
			urlRequest.AddUrlSegment("slug", ApplicationId);

			var federatedCredentials = _restClient.Execute<FederatedUploadCredentials>(urlRequest);
			return federatedCredentials.Data;
		}

		private void TriggerAppHarborBuild(FederatedUploadCredentials credentials, string accessToken)
		{
			_writer.WriteLine("The package will be deployed to application \"{0}\".", ApplicationId);

			if (string.IsNullOrEmpty(_message))
			{
				_message = string.Format("CI Deployment at {0}", DateTime.Now);
			}

			var request = new RestRequest("applications/{slug}/buildnotifications", Method.POST)
			{
				RequestFormat = DataFormat.Json
			}
				.AddUrlSegment("slug", ApplicationId)
				.AddHeader("Authorization", string.Format("BEARER {0}", accessToken))
				.AddBody(new
				{
					Bucket = credentials.Bucket,
					ObjectKey = credentials.ObjectKey,
					CommitMessage = string.IsNullOrEmpty(_message) ? "Deployed from CLI" : _message,
				});

			_writer.WriteLine("Notifying AppHarbor.");

			var response = _restClient.Execute(request);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				using (new ForegroundColor(ConsoleColor.Green))
				{
					_writer.WriteLine("Deploying... Open application overview with `appharbor open`.");
				}
			}
		}

		private string GetAccessToken()
		{
			// Request new access token using the specific
			string accessToken = AccessTokenHelper.GetAccessToken(_username, _password);
			_writer.WriteLine("Logged in with the username " + _username);

			return accessToken;
		}
	}
}
