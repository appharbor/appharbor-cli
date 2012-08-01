using System;
using System.IO;
using System.IO.Compression;
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

			_writer.Write("Getting authorized upload URL... ");

			var presignedUrl = client.Execute(urlRequest).Content;

			var sourceDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

			HttpWebRequest httpRequest = WebRequest.Create(presignedUrl) as HttpWebRequest;
			httpRequest.Method = "PUT";
			httpRequest.AllowWriteStreamBuffering = false;

			var timeout = (int)TimeSpan.FromHours(2).TotalMilliseconds;
			httpRequest.Timeout = timeout;
			httpRequest.ReadWriteTimeout = timeout;

			using (var memoryStream = new MemoryStream())
			{
				using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
				{
					_writer.WriteLine("Preparing deployment package for upload");
					sourceDirectory.ToTar(gzipStream);

					httpRequest.ContentLength = memoryStream.Length;
					using (var uploadStream = httpRequest.GetRequestStream())
					{
						var buffer = new byte[4096];
						memoryStream.Position = 0;

						_writer.WriteLine("Uploading package (total size is {0} MB)",  Math.Round((decimal)memoryStream.Length / 1048576, 2));
						_writer.WriteLine();

						while (true)
						{
							var progressPercentage = (int)((memoryStream.Position * 100) / memoryStream.Length);
							RenderConsoleProgress(progressPercentage, '\u2592', ConsoleColor.Green, string.Format("Uploading ({0}%)", progressPercentage));

							var bytesRead = memoryStream.Read(buffer, 0, buffer.Length);
							if (bytesRead > 0)
							{
								uploadStream.Write(buffer, 0, bytesRead);
							}
							else
							{
								break;
							}
						}
						Console.Write(" ");
					}
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

		private bool TriggerAppHarborBuild(string applicationSlug, string downloadUrl)
		{
			var client = new RestClient("https://packageclient.apphb.com/");

			_writer.Write("Write a deployment message: ");
			var commitMessage = _reader.ReadLine();

			var request = new RestRequest("applications/{slug}/buildnotifications", Method.POST)
			{
				RequestFormat = DataFormat.Json
			}
				.AddUrlSegment("slug", applicationSlug)
				.AddHeader("Authorization", "BEARER " + _accessToken)
				.AddBody(new
				{
					UploadUrl = downloadUrl,
					CommitMessage = commitMessage,
				});

			_writer.WriteLine("Notifying AppHarbor.");

			var response = client.Execute(request);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				_writer.WriteLine("AppHarbor notified, deploying... Open overview in browser with `appharbor open`.");
			}
			return true;
		}

		private static void RenderConsoleProgress(int percentage, char progressBarCharacter, ConsoleColor color, string message)
		{
			ConsoleColor originalColor = Console.ForegroundColor;
			Console.CursorLeft = 0;

			try
			{
				Console.CursorVisible = false;
				Console.ForegroundColor = color;

				int width = Console.WindowWidth - 1;
				int newWidth = (int)((width * percentage) / 100d);
				string progressBar = new string(progressBarCharacter, newWidth) +
					  new string(' ', width - newWidth);

				Console.Write(progressBar);
				if (string.IsNullOrEmpty(message))
				{
					message = "";
				}

				try
				{
					Console.CursorTop++;
				}
				catch (ArgumentOutOfRangeException)
				{
				}

				OverwriteConsoleMessage(message);
				Console.CursorTop--;
			}
			finally
			{
				Console.ForegroundColor = originalColor;
				Console.CursorVisible = true;
			}
		}

		private static void OverwriteConsoleMessage(string message)
		{
			Console.CursorLeft = 0;
			int maxCharacterWidth = Console.WindowWidth - 1;
			if (message.Length > maxCharacterWidth)
			{
				message = message.Substring(0, maxCharacterWidth - 3) + "...";
			}
			message = message + new string(' ', maxCharacterWidth - message.Length);
			Console.Write(message);
		}
	}
}
