using System.IO;
using AppHarbor.Commands;
using AppHarbor.Model;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests.Commands
{
	public class UserCommandTest
	{
		[Theory, AutoCommandData]
		public void ShouldAddConfigurationVariables([Frozen]Mock<IAppHarborClient> client,
			[Frozen]Mock<TextWriter> writer,
			UserCommand command, User user, string applicationId)
		{
			client.Setup(x => x.GetUser()).Returns(user);

			command.Run(new string[0]);

			writer.Verify(x => x.WriteLine("Username: {0}", user.Username));
			writer.Verify(x => x.WriteLine("Email addresses: [{0}]", string.Join(" ", user.Email_Addresses)));
		}
	}
}
