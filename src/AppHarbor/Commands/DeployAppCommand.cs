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
	public class DeployAppCommand : ICommand
	{
		private readonly string _accessToken;
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly TextReader _reader;
		private readonly TextWriter _writer;

		public DeployAppCommand(IApplicationConfiguration applicationConfiguration, IAccessTokenConfiguration accessTokenConfiguration, TextReader reader, TextWriter writer)
		{
			_accessToken = accessTokenConfiguration.GetAccessToken();
			_applicationConfiguration = applicationConfiguration;
			_reader = reader;
			_writer = writer;
		}

		private class TemporaryFileStream : FileStream
		{
			public TemporaryFileStream()
				: base(Path.GetTempFileName(), FileMode.Create, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.DeleteOnClose)
			{
			}
		}

		public void Execute(string[] arguments)
		{
			var uploadCredentials = GetCredentials();
			using (var packageStream = new TemporaryFileStream())
			{
				using (var gzipStream = new GZipStream(packageStream, CompressionMode.Compress, true))
				{
					_writer.WriteLine("Preparing deployment package for upload.");

					var sourceDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
					sourceDirectory.ToTar(gzipStream, excludedDirectoryNames: new[] { ".git", ".hg" });

					using (var s3Client = new AmazonS3Client(uploadCredentials.GetSessionCredentials()))
					{
						var uploadParts = 5;
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
						request.UploadProgressEvent += RenderProgress;

						transferUtility.Upload(request);
					}
				}
			}

			Console.CursorTop++;
			_writer.WriteLine();
			_writer.WriteLine("Package successfully uploaded.");

			TriggerAppHarborBuild(_applicationConfiguration.GetApplicationId(), uploadCredentials);
		}

		private FederatedUploadCredentials GetCredentials()
		{
			var client = new RestClient("https://packageclient.apphb.com/");
			var urlRequest = new RestRequest("applications/{slug}/uploadCredentials", Method.POST);
			urlRequest.AddUrlSegment("slug", _applicationConfiguration.GetApplicationId());

			_writer.WriteLine("Getting upload credentials... ");

			var federatedCredentials = client.Execute<FederatedUploadCredentials>(urlRequest);
			return federatedCredentials.Data;
		}

		private KeyValuePair<DateTime, double> lastUploadProgressEvent;
		private IList<double> bytesPerSecondAverages = new List<double>();

		private void RenderProgress(object sender, UploadProgressArgs uploadProgressArgs)
		{
			var secondsSinceLastAverage = (DateTime.Now - lastUploadProgressEvent.Key).TotalSeconds;

			if (secondsSinceLastAverage > 2)
			{
				if (lastUploadProgressEvent.Key != DateTime.MinValue)
				{
					var bytesSinceLastProgress = uploadProgressArgs.TransferredBytes - lastUploadProgressEvent.Value;

					var bytesPerSecond = bytesSinceLastProgress / secondsSinceLastAverage;
					bytesPerSecondAverages.Add(bytesPerSecond);
				}
				lastUploadProgressEvent = new KeyValuePair<DateTime, double>(DateTime.Now, uploadProgressArgs.TransferredBytes);
			}

			if (bytesPerSecondAverages.Count() > 20)
			{
				bytesPerSecondAverages.RemoveAt(0);
			}

			var bytesRemaining = uploadProgressArgs.TotalBytes - uploadProgressArgs.TransferredBytes;

			var timeEstimate = bytesPerSecondAverages.Count() < 1 ? "Estimating time left" :
				string.Format("Time left: {0:0.0} min",  bytesRemaining / WeightedAverage(bytesPerSecondAverages) / TimeSpan.FromMinutes(1).TotalSeconds);

			ConsoleProgressBar.Render(uploadProgressArgs.PercentDone, ConsoleColor.Green,
				string.Format("Uploading package ({0}% of {1:0.0} MB). {2}",
				uploadProgressArgs.PercentDone, uploadProgressArgs.TotalBytes / 1048576, timeEstimate));
		}

		private static double WeightedAverage(IList<double> input, int spread = 40)
		{
			if (input.Count == 1)
			{
				return input.Average();
			}

			var weightdifference = spread / (input.Count() - 1);
			var averageWeight = 50;
			var startWeight = averageWeight - spread / 2;

			return input.Select((x, i) => x * (startWeight + (i * weightdifference)))
				.Sum() / (averageWeight * input.Count());
		}

		private bool TriggerAppHarborBuild(string applicationSlug, FederatedUploadCredentials credentials)
		{
			var client = new RestClient("https://packageclient.apphb.com/");

			_writer.WriteLine("The package will be deployed to application \"{0}\".", _applicationConfiguration.GetApplicationId());
			_writer.Write("Enter a deployment message:");
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

			var response = client.Execute(request);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				_writer.WriteLine("AppHarbor notified, deploying... Open application overview with `appharbor open`.");
			}
			return true;
		}
	}
}
