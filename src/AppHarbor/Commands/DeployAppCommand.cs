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

		private class TemporaryFileStream : FileStream
		{
			public TemporaryFileStream()
				: base(Path.GetTempFileName(), FileMode.Create, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.DeleteOnClose)
			{
			}
		}

		public void Execute(string[] arguments)
		{
			_writer.WriteLine("Getting upload credentials... ");

			var uploadCredentials = GetCredentials();
			using (var packageStream = new TemporaryFileStream())
			{
				using (var gzipStream = new GZipStream(packageStream, CompressionMode.Compress, true))
				{
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
			var urlRequest = new RestRequest("applications/{slug}/uploadCredentials", Method.POST);
			urlRequest.AddUrlSegment("slug", _applicationConfiguration.GetApplicationId());

			var federatedCredentials = _restClient.Execute<FederatedUploadCredentials>(urlRequest);
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
				string.Format("{0:0.0} left",  GetHumanizedTime(bytesRemaining / WeightedAverage(bytesPerSecondAverages)));

			ConsoleProgressBar.Render(uploadProgressArgs.PercentDone, ConsoleColor.Green,
				string.Format("Uploading package ({0}% of {1:0.0} MB). {2}",
				uploadProgressArgs.PercentDone, uploadProgressArgs.TotalBytes / 1048576, timeEstimate));
		}

		private static string GetHumanizedTime(double seconds)
		{
			string unit;
			int dividend;
			if (seconds < 60)
			{
				unit = "second";
				dividend = 1;
			}
			else if (seconds < 60 * 60)
			{
				unit = "minute";
				dividend = 60;
			}
			else
			{
				unit = "hour";
				dividend = 60 * 60;
			}

			var unitCount = (int)seconds / dividend;
			var approximateValue = ((int)Math.Round(unitCount / 10.0)) * 10;

			var displayValue = unitCount < 10 ? unitCount : approximateValue;
			unit = displayValue == 1 ? unit : unit + "s";


			return string.Format("{0} {1}", displayValue, unit);
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

			var response = _restClient.Execute(request);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				_writer.WriteLine("AppHarbor notified, deploying... Open application overview with `appharbor open`.");
			}
			return true;
		}
	}
}
