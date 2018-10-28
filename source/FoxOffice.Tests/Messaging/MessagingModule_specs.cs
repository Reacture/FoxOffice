namespace FoxOffice.Messaging
{
    using System;
    using Autofac;
    using FluentAssertions;
    using FoxOffice.Domain;
    using FoxOffice.ReadModel;
    using Khala.Messaging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using static Storage;

    [TestClass]
    public class MessagingModule_specs
    {
        [TestMethod]
        public void sut_inherits_Module()
        {
            typeof(MessagingModule).Should().BeDerivedFrom<Module>();
        }

        private static IContainer BuildContainer(
            Action<ContainerBuilder> register = default)
        {
            var builder = new ContainerBuilder();

            register?.Invoke(builder);

            var module = new MessagingModule(
                ConnectionString, MessageQueueName);

            builder.RegisterModule(module);

            return builder.Build();
        }

        [TestMethod]
        public void sut_registers_IMessageSerializer_service_correctly()
        {
            IContainer container = BuildContainer();
            container.AssertServiceRegistered
                <IMessageSerializer, JsonMessageSerializer>();
        }

        [TestMethod]
        public void sut_registers_IMessageBus_service_correctly()
        {
            IContainer container = BuildContainer();
            container.AssertServiceRegistered<IMessageBus, QueueMessageBus>();
        }

        [TestMethod, AutoData]
        public void sut_registers_IMessageHandler_service_correctly(
            TheaterCommandHandler theaterCommandHandler,
            TheaterReadModelGenerator theaterReadModelGenerator,
            MovieCommandHandler movieCommandHandler,
            MovieReadModelGenerator movieReadModelGenerator)
        {
            IContainer container = BuildContainer(builder =>
            {
                builder.RegisterInstance(theaterCommandHandler);
                builder.RegisterInstance(theaterReadModelGenerator);
                builder.RegisterInstance(movieCommandHandler);
                builder.RegisterInstance(movieReadModelGenerator);
            });

            container.AssertServiceRegistered
                <IMessageHandler, CompositeMessageHandler>();

            var service =
                (CompositeMessageHandler)container.Resolve<IMessageHandler>();

            service.Handlers.Should().Contain(theaterCommandHandler);
            service.Handlers.Should().Contain(theaterReadModelGenerator);
            service.Handlers.Should().Contain(movieCommandHandler);
            service.Handlers.Should().Contain(movieReadModelGenerator);
        }
    }
}
