using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using Amazon.S3;
using Amazon.S3.Model;
using RestSharp;

namespace AppHarbor.Commands
{
	[CommandHelp("Deploy current directory", alias: "deploy")]
	public class DeployAppCommand : ICommand
	{
		private readonly string _accessToken;
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly TextReader _reader;
		private readonly ConcurrentDictionary<object, long> _uploadPartRequestProgress = new ConcurrentDictionary<object, long>();
		private readonly TextWriter _writer;

		public DeployAppCommand(IApplicationConfiguration applicationConfiguration, IAccessTokenConfiguration accessTokenConfiguration, TextReader reader, TextWriter writer)
		{
			_accessToken = accessTokenConfiguration.GetAccessToken();
			_applicationConfiguration = applicationConfiguration;
			_reader = reader;
			_writer = writer;
		}

		public void Execute(string[] arguments)
		{
			var uploadCredentials = GetCredentials();
			using (var packageStream = new MemoryStream())
			{
				using (var gzipStream = new GZipStream(packageStream, CompressionMode.Compress, true))
				{
					_writer.WriteLine("Preparing deployment package for upload");

					var sourceDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
					sourceDirectory.ToTar(gzipStream, excludedDirectoryNames: new[] { ".git", ".hg" });

					UploadMultipart(packageStream, uploadCredentials);
				}
			}

			_writer.WriteLine("Package successfully uploaded.");
			TriggerAppHarborBuild(_applicationConfiguration.GetApplicationId(), uploadCredentials);
		}

		private void UploadMultipart(Stream inputStream, FederatedUploadCredentials uploadCredentials)
		{
			using (var s3Client = new AmazonS3Client(uploadCredentials.GetSessionCredentials()))
			{
				var initiateMultipartUploadRequest = new InitiateMultipartUploadRequest
				{
					BucketName = uploadCredentials.Bucket,
					Key = uploadCredentials.ObjectKey,
				};

				var initMultipartUploadResponse = s3Client.InitiateMultipartUpload(initiateMultipartUploadRequest);

				var uploadResponses = new ConcurrentBag<UploadPartResponse>();
				var waitHandlers = new List<WaitHandle>();

				inputStream.Position = 0;
				var partSize = inputStream.Length / 5;

				for (int i = 1; inputStream.Position < inputStream.Length; i++)
				{
					var uploadStream = new MemoryStream();

					while (inputStream.Position < partSize * i)
					{
						var buffer = new byte[4096];
						var bytesRead = inputStream.Read(buffer, 0, buffer.Length);
						if (bytesRead > 0)
						{
							uploadStream.Write(buffer, 0, bytesRead);
						}
						else
						{
							break;
						}
					}

					uploadStream.Position = 0;
					UploadPartRequest uploadRequest = new UploadPartRequest
					{
						BucketName = uploadCredentials.Bucket,
						InputStream = uploadStream,
						Key = uploadCredentials.ObjectKey,
						PartNumber = i,
						PartSize = partSize,
						UploadId = initMultipartUploadResponse.UploadId,
					};

					uploadRequest.WithSubscriber(UploadProgressHandler);
					var asyncResult = s3Client.BeginUploadPart(uploadRequest, x =>
					{
						var response = s3Client.EndUploadPart(x);
						uploadResponses.Add(response);

						uploadStream.Dispose();
					}, uploadRequest);

					waitHandlers.Add(asyncResult.AsyncWaitHandle);
				}

				while (!WaitHandle.WaitAll(waitHandlers.ToArray(), 500))
				{
					var progressPercentage = 100 * _uploadPartRequestProgress.Sum(x => x.Value) / (double)inputStream.Length;
					ConsoleProgressBar.Render(progressPercentage, ConsoleColor.Green,
						string.Format("Uploading page ({0:0.0}% of {1:0.0} MB).",
						progressPercentage, inputStream.Length / 1048576));
				}

				var completeMultipartUploadRequest = new CompleteMultipartUploadRequest
				{
					BucketName = uploadCredentials.Bucket,
					Key = uploadCredentials.ObjectKey,
					PartETags = uploadResponses.Select(x => new PartETag(x.PartNumber, x.ETag)).ToList(),
					UploadId = initMultipartUploadResponse.UploadId,
				};

				s3Client.CompleteMultipartUpload(completeMultipartUploadRequest);
			}
		}

		public void UploadProgressHandler(object request, UploadPartProgressArgs foobar)
		{
			_uploadPartRequestProgress.AddOrUpdate(request, foobar.TransferredBytes,
				(x, i) => foobar.TransferredBytes);
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

		private static double WeightedAverage(IList<double> input, int spread = 40)
		{
			if (input.Count < 2)
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
