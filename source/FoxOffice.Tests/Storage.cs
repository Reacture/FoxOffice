namespace FoxOffice
{
    using System.Threading.Tasks;

    public static class Storage
    {
        public const string ConnectionString = "UseDevelopmentStorage=true";
        public const string MessageQueueName = "foxoffice-test-messages";
        public const string EventStoreTableName = "FoxOfficeTestEventStore";
    }
}
