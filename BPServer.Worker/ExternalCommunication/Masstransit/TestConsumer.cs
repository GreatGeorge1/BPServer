using BPSever.Infrastracture.MessageTypes;
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
        private readonly IRequestClient<TestRequest> _client;

        public TestConsumer(ILogger logger, IRequestClient<TestRequest> client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task Consume(ConsumeContext<TestEvent> context)
        {
            _logger.Information($"Masstransit test:'{context.Message.Text}, {context.MessageId}'");
            var response=
                await _client.GetResponse<TestRequestResult>(new { Text = "some_text", Id = Guid.NewGuid().ToString() });
            _logger.Information($"Masstransit test response:'{response.Message.Result}, {response.Message.Id}'");

        }
    }

}

