using BPServer.Core.Handlers;
using BPServer.Core.Messages;
using System;
using System.Timers;

namespace BPServer.Core.Sagas
{
    public abstract class Saga : ISaga
    {
        public string TransportName { get; protected set; }

        public ICommand Command { get; protected set; }

        public Guid Id { get; protected set; }

        public bool IsError { get; protected set; }

        public bool IsTimeout { get; protected set; }

        public bool IsCompleted { get; protected set; }

        public bool IsRepeatLimitReached { get; protected set; }

        public TimeSpan Timeout { get; protected set; }

        public int MaxRepeats { get; protected set; }

        public int RepeatCount { get; protected set; } 

        public DateTime CreationTime { get; protected set; }

        public IMessage Ack { get; protected set; }

        public IMessage RequestMessage { get; protected set; }

        public IMessage SentResponse { get; protected set; }

        public bool HasResponse { get; protected set; }

        public IMessage CommandMessage { get; protected set; }

        public IMessage RecievedResponse { get; protected set; }

        private Timer _timer;

        protected Saga(string transportName, ICommand command, TimeSpan timeout, int maxRepeats, bool hasCommandResponse)
        {
            if (string.IsNullOrWhiteSpace(transportName))
            {
                throw new ArgumentException("message", nameof(transportName));
            }

            Id = Guid.NewGuid();
            CreationTime = DateTime.UtcNow;
            RepeatCount = 0;
            TransportName = transportName;
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Timeout = timeout;
            MaxRepeats = maxRepeats;
            HasResponse = hasCommandResponse;
            if (Timeout.TotalMilliseconds != 0)
            {
                _timer = new Timer();
                _timer.Enabled = true;
                _timer.Interval = Timeout.TotalMilliseconds;
                _timer.Elapsed += _timer_Elapsed;
                _timer.Start();
            }
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnTimeoutReached(Id);
        }

        public event EventHandler<Guid> RepeatLimitReached;
        public event EventHandler<Guid> Completed;
        public event EventHandler<Guid> Error;
        public event EventHandler<Guid> TimeoutReached;

        protected virtual void OnCompleted(Guid e)
        {
            EventHandler<Guid> handler = Completed;
            handler?.Invoke(this, e);
        }

        protected virtual void OnRepeatLimitReached(Guid e)
        {
            EventHandler<Guid> handler = RepeatLimitReached;
            handler?.Invoke(this, e);
        }

        protected virtual void OnTimeoutReached(Guid e)
        {
            EventHandler<Guid> handler = TimeoutReached;
            _timer.Stop();
            handler?.Invoke(this, e);
        }

        protected virtual void OnError(Guid e)
        {
            EventHandler<Guid> handler = Error;
            if (Timeout.TotalMilliseconds != 0) _timer.Stop();
            handler?.Invoke(this, e);
        }

        public void RepeatIncrement()
        {
            if ((RepeatCount + 1) >= MaxRepeats)
            {
                IsRepeatLimitReached = true;
                OnRepeatLimitReached(Id);
            }
            else
            {
                RepeatCount++;
                IsTimeout = false;
                IsError = false;
                if (Timeout.TotalMilliseconds > 0)
                {
                    _timer = new Timer();
                    _timer.Interval = Timeout.TotalMilliseconds;
                    _timer.Start();
                }
            }
        }

        public void SetAck(IMessage message)
        {
            Ack = message ?? throw new ArgumentNullException(nameof(message));
        }

        public void SetResponse(IMessage recievedResponse)
        {
            RecievedResponse = recievedResponse ?? throw new ArgumentNullException(nameof(recievedResponse));
        }

        public void SetCompleted()
        {
            IsCompleted = true;
            if (Timeout.TotalMilliseconds != 0) _timer.Stop();
            OnCompleted(Id);
        }

        public void SetError()
        {
            IsError = true;
            OnError(Id);
        }

        public void SetSentResponse(IMessage message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            SentResponse = message;
        }

        public void SetRequestMesssage(IMessage message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.RequestMessage = message;
        }

        public void SetCommandMessage(IMessage message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            CommandMessage = message;
        }
    }
}
