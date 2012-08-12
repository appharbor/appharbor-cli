using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using Amazon.S3;
using Amazon.S3.Transfer;
using RestSharp;

namespace AppHarbor.Commands
{
	[CommandHelp("Deploy current directory", alias: "deploy")]
	public class DeployAppCommand : ICommand
	{
		private readonly string _accessToken;
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly IRestClient _restClient;
		private readonly TextReader _reader;
		private readonly TextWriter _writer;

		public DeployAppCommand(IApplicationConfiguration applicationConfiguration, IAccessTokenConfiguration accessTokenConfiguration, TextReader reader, TextWriter writer)
		{
			_accessToken = accessTokenConfiguration.GetAccessToken();
			_applicationConfiguration = applicationConfiguration;
			_restClient = new RestClient("https://packageclient.apphb.com/");
			_reader = reader;
			_writer = writer;
		}

		public void Execute(string[] arguments)
		{
			_writer.WriteLine("Getting upload credentials... ");
			_writer.WriteLine();

			var uploadCredentials = GetCredentials();
			using (var packageStream = new TemporaryFileStream())
			{
				using (var gzipStream = new GZipStream(packageStream, CompressionMode.Compress, true))
				{
					var sourceDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
					sourceDirectory.ToTar(gzipStream, excludedDirectoryNames: new[] { ".git", ".hg" });

					using (var s3Client = new AmazonS3Client(uploadCredentials.GetSessionCredentials()))
					{
						const int uploadParts = 5;
						var partSize = packageStream.Length / uploadParts;

						var transferUtilityConfig = new TransferUtilityConfig();
						transferUtilityConfig.NumberOfUploadThreads = uploadParts * 2;

						var transferUtility = new TransferUtility(s3Client, transferUtilityConfig);
						packageStream.Position = 0;
						var request = new TransferUtilityUploadRequest
						{
							InputStream = packageStream,
							BucketName = uploadCredentials.Bucket,
							Key = uploadCredentials.ObjectKey,
							PartSize = partSize,
						};
						var progressBar = new ConsoleProgressBar();
						request.UploadProgressEvent += (x, y) =>
							progressBar.RenderProgress("Uploading package", "MB", y.TransferredBytes / 1048576, y.TotalBytes / 1048576);

						transferUtility.Upload(request);

						Console.CursorTop++;
						_writer.WriteLine();
					}
				}
			}

			TriggerAppHarborBuild(_applicationConfiguration.GetApplicationId(), uploadCredentials);
		}

		private FederatedUploadCredentials GetCredentials()
		{
			var urlRequest = new RestRequest("applications/{slug}/uploadCredentials", Method.POST);
			urlRequest.AddUrlSegment("slug", _applicationConfiguration.GetApplicationId());

			var federatedCredentials = _restClient.Execute<FederatedUploadCredentials>(urlRequest);
			return federatedCredentials.Data;
		}

		private void TriggerAppHarborBuild(string applicationSlug, FederatedUploadCredentials credentials)
		{
			_writer.WriteLine("The package will be deployed to application \"{0}\".", _applicationConfiguration.GetApplicationId());

			using (new ForegroundColor(ConsoleColor.Yellow))
			{
				_writer.WriteLine();
				_writer.Write("Enter a deployment message: ");
			}
			var commitMessage = _reader.ReadLine();

			var request = new RestRequest("applications/{slug}/buildnotifications", Method.POST)
			{
				RequestFormat = DataFormat.Json
			}
				.AddUrlSegment("slug", applicationSlug)
				.AddHeader("Authorization", string.Format("BEARER {0}", _accessToken))
				.AddBody(new
				{
					Bucket = credentials.Bucket,
					ObjectKey = credentials.ObjectKey,
					CommitMessage = commitMessage,
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
	}
}
