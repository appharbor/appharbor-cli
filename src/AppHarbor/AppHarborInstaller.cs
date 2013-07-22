using System;
using System.Collections.Generic;
using System.IO;
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
				.BasedOn<Command>());

			container.Register(Component
				.For<IEnumerable<Type>>()
				.UsingFactoryMethod(x =>
				{
					return Assembly.GetExecutingAssembly().GetExportedTypes()
						.Where(y => typeof(Command).IsAssignableFrom(y) && y.IsClass && !y.IsAbstract);
				}));

			container.Register(Component
				.For<TextWriter>()
				.UsingFactoryMethod(x =>
				{
					return Console.Out;
				}));

			container.Register(Component
				.For<TextReader>()
				.UsingFactoryMethod(x =>
				{
					return Console.In;
				}));

			container.Register(Component
				.For<IAccessTokenConfiguration>()
				.ImplementedBy<AccessTokenConfiguration>());

			container.Register(Component
				.For<IAppHarborClient>()
				.UsingFactoryMethod(x =>
				{
					var accessTokenConfiguration = container.Resolve<IAccessTokenConfiguration>();
					return new AppHarborCliClient(accessTokenConfiguration.GetAccessToken());
				}));

			container.Register(Component
				.For<IApplicationConfiguration>()
				.ImplementedBy<ApplicationConfiguration>());

			container.Register(Component
				.For<CommandDispatcher>());

			container.Register(Component
				.For<ITypeNameMatcher>()
				.ImplementedBy<TypeNameMatcher<Command>>());

			container.Register(Component
				.For<IAliasMatcher>()
				.ImplementedBy<AliasMatcher>());

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

			container.Register(Component
				.For<IMaskedInput>()
				.ImplementedBy<MaskedConsoleInput>());

			RegisterProgressBar(container);
		}

		private static void RegisterProgressBar(IWindsorContainer container)
		{
			if (ConsoleWindowHelper.HasConsoleWindow)
			{
				container.Register(Component
					.For<IProgressBar>()
					.ImplementedBy<MegaByteProgressBar>()
					.LifeStyle.Transient);
			}
			else
			{
				container.Register(Component
					.For<IProgressBar>()
					.ImplementedBy<NullProgressBar>()
					.LifeStyle.Transient);
			}
		}
	}
}
