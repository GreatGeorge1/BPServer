using BPSever.Infrastracture.MessageTypes;
using MassTransit;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPServer.Master
{
    public class TestRequestConsumer : IConsumer<TestRequest>
    {
        private readonly ILogger _logger;

        public TestRequestConsumer(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<TestRequest> context)
        {
            _logger.Information($"Consumed message '{context.MessageId}, {context.Message.Text}, {context.Message.Id}'");
            await context.RespondAsync<TestRequestResult>(new 
            {
                Result="some_result",
                context.Message.Id
            });
        }
    }

}
