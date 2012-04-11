using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace AppHarbor
{
	public class AppHarborInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));

			container.Register(AllTypes.FromThisAssembly()
				.BasedOn<ICommand>());

			container.Register(Component
				.For<IEnumerable<Type>>()
				.UsingFactoryMethod(x =>
				{
					return Assembly.GetExecutingAssembly().GetExportedTypes()
						.Where(y => typeof(ICommand).IsAssignableFrom(y) && y.IsClass);
				}));

			container.Register(Component
				.For<IAccessTokenConfiguration>()
				.ImplementedBy<AccessTokenConfiguration>());

			container.Register(Component
				.For<IAppHarborClient>()
				.UsingFactoryMethod(x =>
				{
					var accessTokenConfiguration = container.Resolve<IAccessTokenConfiguration>();
					return new AppHarborClient(accessTokenConfiguration.GetAccessToken());
				}));

			container.Register(Component
				.For<IApplicationConfiguration>()
				.ImplementedBy<ApplicationConfiguration>());

			container.Register(Component
				.For<CommandDispatcher>());

			container.Register(Component
				.For<ITypeNameMatcher>()
				.ImplementedBy<TypeNameMatcher<ICommand>>());

			container.Register(Component
				.For<IGitCommand>()
				.ImplementedBy<GitCommand>());

			container.Register(Component
				.For<IGitRepositoryConfigurer>()
				.ImplementedBy<GitRepositoryConfigurer>());

			container.Register(Component
				.For<IFileSystem>()
				.ImplementedBy<PhysicalFileSystem>()
				.LifeStyle.Transient);
		}
	}
}
