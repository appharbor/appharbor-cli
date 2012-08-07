using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
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

		public void Execute(string[] arguments)
		{
			var client = new RestClient("https://packageclient.apphb.com/");
			var urlRequest = new RestRequest("applications/{slug}/authenticatedurls", Method.POST);
			urlRequest.AddUrlSegment("slug", _applicationConfiguration.GetApplicationId());

			_writer.WriteLine("Getting package upload URL... ");

			var presignedUrl = client.Execute(urlRequest).Content;

			var sourceDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

			HttpWebRequest httpRequest = WebRequest.Create(presignedUrl) as HttpWebRequest;
			httpRequest.Method = "PUT";
			httpRequest.AllowWriteStreamBuffering = false;

			var timeout = (int)TimeSpan.FromHours(2).TotalMilliseconds;
			httpRequest.Timeout = timeout;
			httpRequest.ReadWriteTimeout = timeout;

			using (var packageStream = new MemoryStream())
			{
				using (var gzipStream = new GZipStream(packageStream, CompressionMode.Compress, true))
				{
					_writer.WriteLine("Preparing deployment package for upload");
					sourceDirectory.ToTar(gzipStream, excludedDirectoryNames: new[] { ".git", ".hg" });

					PerformUpload(httpRequest, packageStream);
				}
			}
			var response = (HttpWebResponse)httpRequest.GetResponse();

			if (response.StatusCode != HttpStatusCode.OK)
			{
				_writer.WriteLine("Package upload failed. Please try again.");
				return;
			}

			_writer.WriteLine("Package successfully uploaded.");
			TriggerAppHarborBuild(_applicationConfiguration.GetApplicationId(), presignedUrl);
		}

		private void PerformUpload(HttpWebRequest httpRequest, MemoryStream inputStream)
		{
			httpRequest.ContentLength = inputStream.Length;

			using (var uploadStream = httpRequest.GetRequestStream())
			{
				var buffer = new byte[65536];
				inputStream.Position = 0;

				DateTime time = DateTime.Now;
				int i = 0;
				double bytesPerSecond = 0;
				double timeleft = 0;
				var averages = new List<double>();

				while (true)
				{
					var timedifference = (DateTime.Now - time).TotalSeconds;
					if (timedifference > 2)
					{
						bytesPerSecond = ((buffer.Length * i) / timedifference);
						averages.Add(bytesPerSecond);
						time = DateTime.Now;
						i = 0;
					}

					if (averages.Count > 0)
					{
						timeleft = (inputStream.Length - inputStream.Position) / WeightedAverage(averages) / 60;
					}

					if (averages.Count > 20)
					{
						averages.RemoveAt(0);
					}

					var progressPercentage = (double)(inputStream.Position * 100) / inputStream.Length;
					ConsoleProgressBar.Render(progressPercentage, ConsoleColor.Green,
						string.Format("Uploading page ({0:0.0}% of {1:0.0} MB). Time left: {2:0.0} minutes",
						progressPercentage, (decimal)inputStream.Length / 1048576, timeleft));

					var bytesRead = inputStream.Read(buffer, 0, buffer.Length);
					if (bytesRead > 0)
					{
						uploadStream.Write(buffer, 0, bytesRead);
					}
					else
					{
						break;
					}
					i++;
				}
				_writer.WriteLine();
			}
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

		private bool TriggerAppHarborBuild(string applicationSlug, string downloadUrl)
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
					UploadUrl = downloadUrl,
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
