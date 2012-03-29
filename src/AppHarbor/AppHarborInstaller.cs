using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace AppHarbor
{
	public class AppHarborInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Component
				.For<AuthInfo>()
				.UsingFactoryMethod(x =>
				{
					var token = Environment.GetEnvironmentVariable("AppHarborToken", EnvironmentVariableTarget.User);
					return new AuthInfo { AccessToken = token };
				}));
		}
	}
}
