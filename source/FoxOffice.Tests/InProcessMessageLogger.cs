namespace FoxOffice
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Khala.Messaging;

    public class InProcessMessageLogger : IMessageBus
    {
        private readonly ConcurrentQueue<Envelope> _log;

        public InProcessMessageLogger()
            => _log = new ConcurrentQueue<Envelope>();

        public IEnumerable<Envelope> Log => _log;

        public async Task Send(
            Envelope envelope, CancellationToken cancellationToken)
        {
            await Task.Delay(millisecondsDelay: 1);
            _log.Enqueue(envelope);
        }

        public async Task Send(
            IEnumerable<Envelope> envelopes,
            CancellationToken cancellationToken)
        {
            await Task.Delay(millisecondsDelay: 1);
            foreach (Envelope envelope in envelopes)
            {
                _log.Enqueue(envelope);
            }
        }

        public void ClearLog() => _log.Clear();
    }
}
