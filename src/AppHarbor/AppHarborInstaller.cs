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
				.For<AccessTokenConfiguration>());

			container.Register(Component
				.For<AppHarborClient>()
				.UsingFactoryMethod(x =>
				{
					var accessTokenConfiguration = container.Resolve<AccessTokenConfiguration>();
					return new AppHarborClient(accessTokenConfiguration.GetAccessToken());
				}));

			container.Register(Component
				.For<CommandDispatcher>());
		}
	}
}
