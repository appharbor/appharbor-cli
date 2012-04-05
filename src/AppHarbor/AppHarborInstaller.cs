using System.Reflection;
using System.Linq;
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
				.For<TypeNameMatcher<ICommand>>()
				.UsingFactoryMethod(x =>
				{
					return new TypeNameMatcher<ICommand>(Assembly.GetExecutingAssembly().GetExportedTypes()
						.Where(y => typeof(ICommand).IsAssignableFrom(y)));
				}));

			container.Register(Component
				.For<IGitExecutor>()
				.ImplementedBy<GitExecutor>());

			container.Register(Component
				.For<IFileSystem>()
				.ImplementedBy<PhysicalFileSystem>()
				.LifeStyle.Transient);
		}
	}
}
