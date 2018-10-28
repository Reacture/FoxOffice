namespace FoxOffice.ReadModel
{
    using Autofac;
    using FluentAssertions;
    using FoxOffice.ReadModel.DataAccess;
    using Microsoft.Azure.Documents;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ReadModelModule_specs
    {
        [TestMethod]
        public void sut_inherits_Module()
        {
            typeof(ReadModelModule).Should().BeDerivedFrom<Module>();
        }

        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            var module = new ReadModelModule(
                CosmosDb.Endpoint,
                CosmosDb.AuthKey,
                CosmosDb.DatabaseId,
                CosmosDb.CollectionId);

            builder.RegisterModule(module);

            return builder.Build();
        }

        [TestMethod]
        public void sut_registers_IDocumentClient_service()
        {
            IContainer container = BuildContainer();
            container.AssertServiceRegistered<IDocumentClient>();
        }

        [TestMethod]
        public void sut_registers_CollectionReference_service()
        {
            IContainer container = BuildContainer();
            container.AssertServiceRegistered<CollectionReference>();
            CollectionReference service =
                container.Resolve<CollectionReference>();
            service.DatabaseId.Should().Be(CosmosDb.DatabaseId);
            service.CollectionId.Should().Be(CosmosDb.CollectionId);
        }

        [TestMethod]
        public void sut_registers_ITheaterRepository_service()
        {
            IContainer container = BuildContainer();
            container.AssertServiceRegistered
                <ITheaterRepository, CosmosDbTheaterRepository>();
        }

        [TestMethod]
        public void sut_registers_ITheaterReader_service()
        {
            IContainer container = BuildContainer();
            container.AssertServiceRegistered
                <ITheaterReader, CosmosDbTheaterRepository>();
        }

        [TestMethod]
        public void sut_registers_TheaterReadModelGenerator_service()
        {
            IContainer container = BuildContainer();
            container.AssertServiceRegistered<TheaterReadModelGenerator>();
        }

        [TestMethod]
        public void sut_registers_TheaterReadModelFacade_service()
        {
            IContainer container = BuildContainer();
            container.AssertServiceRegistered<TheaterReadModelFacade>();
        }

        [TestMethod]
        public void sut_registers_IMovieRepository_service()
        {
            IContainer container = BuildContainer();
            container.AssertServiceRegistered
                <IMovieRepository, CosmosDbMovieRepository>();
        }

        [TestMethod]
        public void sut_registers_MovieReadModelGenerator_service()
        {
            IContainer container = BuildContainer();
            container.AssertServiceRegistered<MovieReadModelGenerator>();
        }

        [TestMethod]
        public void sut_registers_MovieReadModelFacade_service()
        {
            IContainer container = BuildContainer();
            container.AssertServiceRegistered<MovieReadModelFacade>();
        }
    }
}
