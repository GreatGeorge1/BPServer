using BPServer.Core.MessageBus;
using BPServer.Core.MessageBus.Messages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BPServer.Core.Transports
{
    public class TestTransport : ITransport
    {
        private readonly ILogger log;
        private bool isDisposed;

        public string Name => "test_transport";

        private IMessageBus _messageBus;

        public TestTransport(ILogger<TestTransport> logger)
        {
            log = logger ?? throw new ArgumentNullException(nameof(logger));
            Start();
        }
        private async Task Start()
        {
            var len=Message.IntToHighLow(2);
            var body = new byte[] { 0x01, 0x00 };
            var mm = new List<byte> { 0x02, 0xd5, 0xc7 };
            mm.Add(Message.CalCheckSum(body));
            mm.AddRange(Message.IntToHighLow(body.Length));
            mm.AddRange(body);
            var message = new NotificationMessage(mm.ToArray());
            await Task.Delay(TimeSpan.FromSeconds(3));
            while (!isDisposed)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                OnDataReceived(message);
               //log.Error("Test mesage pushed");
            }
        }

        protected virtual void OnDataReceived(IMessage e)
        {
            if(_messageBus is null)
            {
                log.LogDebug($"On {Name} messageBus is not set");
                return;
            }
            _messageBus.Publish(e, Name);
        }

        public Task PushDataAsync(IMessage input)
        {
            log.LogDebug("Data pushed");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;
            //Clear();
            isDisposed = true;
        }

        public string GetInfo()
        {
            return Name;
        }

        public void SetMessageBus(IMessageBus messageBus)
        {
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
        }
    }

}
