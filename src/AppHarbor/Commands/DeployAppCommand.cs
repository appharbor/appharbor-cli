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
		private string _message;
		private DirectoryInfo _sourceDirectory;

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

			_sourceDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
			OptionSet.Add("source-directory=", "Set source directory", x =>
			{
				_sourceDirectory = new DirectoryInfo(x);

				if (!_sourceDirectory.Exists)
				{
					throw new CommandException(string.Format("The directory '{0}' does not exist or is not accessible", _sourceDirectory.FullName));
				}
			});

			_excludedDirectories = new List<string> { ".git", ".hg" };
			OptionSet.Add("e|excluded-directory=", "Add excluded directory name", x => _excludedDirectories.Add(x));

			OptionSet.Add("m|message=", "Specify commit message", x => _message = x);
		}

		protected override void InnerExecute(string[] arguments)
		{
			_writer.WriteLine("Getting upload credentials... ");
			_writer.WriteLine();

			var uploadCredentials = GetCredentials();

			var temporaryFileName = Path.GetTempFileName();
			try
			{
				using (var packageStream = new FileStream(temporaryFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
				using (var gzipStream = new GZipStream(packageStream, CompressionMode.Compress, true))
				{
					_sourceDirectory.ToTar(gzipStream, excludedDirectoryNames: _excludedDirectories.ToArray());
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

					var progressBar = new MegaByteProgressBar();
					request.UploadProgressEvent += (object x, UploadProgressArgs y) => progressBar
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

			if (string.IsNullOrEmpty(_message))
			{
				using (new ForegroundColor(ConsoleColor.Yellow))
				{
					_writer.WriteLine();
					_writer.Write("Enter a deployment message: ");
				}
				_message = _reader.ReadLine();
			}

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
	}
}
