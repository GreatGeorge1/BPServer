using BPServer.Core.MessageBus.Messages;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BPServer.Core.Transports
{
    public class TestTransport : ITransport
    {
        private readonly ILogger log;
        public TestTransport(ILogger logger)
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
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
                OnDataReceived(message);
               //log.Error("Test mesage pushed");
            }
        }


        public string Name => "TestTransport";

        public event EventHandler<IMessage> DataReceived;

        protected virtual void OnDataReceived(IMessage e)
        {
            EventHandler<IMessage> handler = DataReceived;
            handler?.Invoke(this, e);
        }

        public string GetInfo()
        {
            return Name;
        }

        public Task PushDataAsync(IMessage input)
        {
            log.Verbose("Data pushed");
            return Task.CompletedTask;
        }
    }
}
