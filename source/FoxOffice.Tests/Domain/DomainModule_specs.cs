namespace FoxOffice.Domain
{
    using Autofac;
    using FluentAssertions;
    using Khala.Messaging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class DomainModule_specs
    {
        [TestMethod]
        public void sut_inherits_Module()
        {
            typeof(DomainModule).Should().BeDerivedFrom<Module>();
        }

        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(Mock.Of<IMessageSerializer>());
            builder.RegisterInstance(Mock.Of<IMessageBus>());

            var module = new DomainModule(
                Storage.ConnectionString, Storage.EventStoreTableName);

            builder.RegisterModule(module);

            return builder.Build();
        }

        [TestMethod]
        public void sut_registers_Theater_factory_service_correctly()
        {
            IContainer container = BuildContainer();
            container.AssertFactoryRegistered(Theater.Factory);
        }

        [TestMethod]
        public void sut_registers_Theater_repository_service()
        {
            IContainer container = BuildContainer();
            container.AssertRepositoryRegistered<Theater>();
        }

        [TestMethod]
        public void sut_registers_TheaterCommandHandler_service()
        {
            IContainer container = BuildContainer();
            container.AssertServiceRegistered<TheaterCommandHandler>();
        }

        [TestMethod]
        public void sut_registers_Movie_factory_service_correctly()
        {
            IContainer container = BuildContainer();
            container.AssertFactoryRegistered(Movie.Factory);
        }

        [TestMethod]
        public void sut_registers_Movie_repository_service()
        {
            IContainer container = BuildContainer();
            container.AssertRepositoryRegistered<Movie>();
        }

        [TestMethod]
        public void sut_registers_MovieCommandHandler_service()
        {
            IContainer container = BuildContainer();
            container.AssertServiceRegistered<MovieCommandHandler>();
        }
    }
}
