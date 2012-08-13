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
	[CommandHelp("Deploy current directory", alias: "deploy")]
	public class DeployAppCommand : ApplicationCommand
	{
		private readonly string _accessToken;
		private readonly IRestClient _restClient;
		private readonly TextReader _reader;
		private readonly TextWriter _writer;

		private readonly IList<string> _excludedDirectories;

		public DeployAppCommand(IApplicationConfiguration applicationConfiguration, IAccessTokenConfiguration accessTokenConfiguration, TextReader reader, TextWriter writer)
			: base(applicationConfiguration)
		{
			_accessToken = accessTokenConfiguration.GetAccessToken();
			_restClient = new RestClient("https://packageclient.apphb.com/");
			_reader = reader;
			_writer = writer;

			_excludedDirectories = new List<string> { ".git", ".hg" };
			OptionSet.Add("e|excluded-directory-name=", "Add excluded directory name", x => _excludedDirectories.Add(x));
		}

		protected override void InnerExecute(string[] arguments)
		{
			_writer.WriteLine("Getting upload credentials... ");
			_writer.WriteLine();

			var uploadCredentials = GetCredentials();
			using (var packageStream = new TemporaryFileStream())
			{
				using (var gzipStream = new GZipStream(packageStream, CompressionMode.Compress, true))
				{
					var sourceDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
					sourceDirectory.ToTar(gzipStream, excludedDirectoryNames: _excludedDirectories.ToArray());

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

						var progressBar = new MegaByteProgressBar();
						request.UploadProgressEvent += (object x, UploadProgressArgs y) => progressBar
							.Update("Uploading package", y.TransferredBytes, y.TotalBytes);

						transferUtility.Upload(request);

						Console.CursorTop++;
						_writer.WriteLine();
					}
				}
			}

			TriggerAppHarborBuild(uploadCredentials);
		}

		private FederatedUploadCredentials GetCredentials()
		{
			var urlRequest = new RestRequest("applications/{slug}/uploadCredentials", Method.POST);
			urlRequest.AddUrlSegment("slug", ApplicationId);

			var federatedCredentials = _restClient.Execute<FederatedUploadCredentials>(urlRequest);
			return federatedCredentials.Data;
		}

		private void TriggerAppHarborBuild(FederatedUploadCredentials credentials)
		{
			_writer.WriteLine("The package will be deployed to application \"{0}\".", ApplicationId);

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
				.AddUrlSegment("slug", ApplicationId)
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
