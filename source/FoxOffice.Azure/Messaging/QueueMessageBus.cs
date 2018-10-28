namespace FoxOffice.Messaging
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Khala.Messaging;
    using Microsoft.WindowsAzure.Storage.Queue;

    public class QueueMessageBus : IMessageBus
    {
        private readonly IMessageSerializer _serializer;
        private readonly CloudQueue _queue;

        public QueueMessageBus(CloudQueue queue)
        {
            _serializer = new JsonMessageSerializer();
            _queue = queue;
        }

        public Task Send(
            Envelope envelope, CancellationToken cancellationToken)
        {
            return SendMessage(envelope);
        }

        public Task Send(
            IEnumerable<Envelope> envelopes,
            CancellationToken cancellationToken)
        {
            return Task.WhenAll(envelopes.Select(SendMessage));
        }

        private Task SendMessage(Envelope envelope)
        {
            string content = _serializer.Serialize(envelope.Message);
            var message = new CloudQueueMessage(content);
            return _queue.AddMessageAsync(message);
        }
    }
}
