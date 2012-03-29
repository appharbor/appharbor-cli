using System;
using System.IO;
using AppHarbor.Commands;
using Xunit;

namespace AppHarbor.Tests.Commands
{
	public class LoginCommandTest
	{
		public void ShouldAskForUsernameAndPassword()
		{
			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);

				using (var reader = new StringReader(string.Format("foo{0}bar{0}", Environment.NewLine)))
				{
					Console.SetIn(reader);

					var loginCommand = new LoginCommand(null);
					loginCommand.Execute(new string[] { });

					var expected = string.Format("Username:{0}Password:{0}", Environment.NewLine);
					Assert.Equal(expected, writer.ToString());
				}
			}
		}
	}
}
