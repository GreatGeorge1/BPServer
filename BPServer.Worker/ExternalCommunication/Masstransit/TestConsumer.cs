using MassTransit;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPServer.Worker.ExternalCommunication.Masstransit
{
    public class TestConsumer : IConsumer<TestEvent>
    {
        private readonly ILogger _logger;

        public TestConsumer(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Consume(ConsumeContext<TestEvent> context)
        {
            _logger.Information($"Masstransit test:'{context.Message.Text}, {context.MessageId}'");
            return Task.CompletedTask;
        }
    }
    public interface TestEvent
    {
        string Text { get; }
    }
}
