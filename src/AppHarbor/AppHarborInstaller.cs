using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;

namespace AppHarbor
{
	public class AppHarborInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));

			container.Register(AllTypes
				.FromThisAssembly()
				.BasedOn<ICommand>()
				.WithService.AllInterfaces());

			container.Register(Component
				.For<AppHarborApi>());

			container.Register(Component
				.For<AuthInfo>()
				.UsingFactoryMethod(x =>
				{
					var token = Environment.GetEnvironmentVariable("AppHarborToken", EnvironmentVariableTarget.User);
					return new AuthInfo { AccessToken = token };
				}));

			container.Register(Component
				.For<CommandDispatcher>());
		}
	}
}
