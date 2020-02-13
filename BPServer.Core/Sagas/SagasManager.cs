using BPServer.Core.Handlers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BPServer.Core.Sagas
{
    public class SagasManager : ISagasManager,IDisposable
    {
        private bool isDisposed;
        protected readonly List<ISaga> _sagas;
        private readonly ILogger log;
        public SagasManager(ILogger logger)
        {
            log = logger ?? throw new ArgumentNullException(nameof(logger));
            _sagas = new List<ISaga>();;
        }

        protected void OnCompleted(object sender, Guid id)
        {
            var saga = _sagas.First(x => x.Id == id);
            if(_sagas.Remove(saga))//TODO save
            {
                saga.Completed -= OnCompleted;
                saga.RepeatLimitReached -= OnRepeatLimitReached;
                saga.TimeoutReached -= OnTimeoutReached;
                saga.Error -= OnError;
            }
            log.Information($"Saga Completed: '{saga.Id}', removed");
        }

        protected void OnError(object sender, Guid id)
        {
            log.Warning($"Saga Error: '{id}'");
            var saga = _sagas.First(x => x.Id == id);
            saga.RepeatIncrement();
        }

        protected void OnTimeoutReached(object sender, Guid id)
        {
            log.Warning($"Saga Timeout: '{id}'");
            var saga = _sagas.First(x => x.Id == id);
            if (saga.IsCompleted)
            {
                return;
            }
            saga.RepeatIncrement();
        }

        protected void OnRepeatLimitReached(object sender, Guid id)
        {
            log.Error($"Saga RepeatLimitReached: '{id}', removed");
            var saga = _sagas.First(x => x.Id == id);
            _sagas.Remove(saga);
        }

        public void AddSaga(ISaga serverSaga)
        {
            serverSaga.Completed += OnCompleted;
            serverSaga.RepeatLimitReached += OnRepeatLimitReached;
            serverSaga.TimeoutReached += OnTimeoutReached;
            serverSaga.Error += OnError;
            _sagas.Add(serverSaga);
        }

        public bool TryGet(Guid id, out ISaga saga)
        {
            var temp = _sagas.FirstOrDefault(x => x.Id == id);
            if(temp is null)
            {
                saga = null;
                return false;
            }
            saga = temp;
            return true;
        }

        public bool TryGet(string serialPort, ICommand command, out ISaga saga)
        {
            var temp = _sagas
                .Where(x => x.TransportName.Equals(serialPort))
                .Where(x => x.Command.Command == command.Command)
                .FirstOrDefault();
            if (temp is null)
            {
                saga = null;
                return false;
            }
            saga = temp;
            return true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;
            if (_sagas.Any())
            {
                foreach(var saga in _sagas)
                {
                    saga.Completed -= OnCompleted;
                    saga.RepeatLimitReached -= OnRepeatLimitReached;
                    saga.TimeoutReached -= OnTimeoutReached;
                    saga.Error -= OnError;
                }
                _sagas.Clear();
            }

            isDisposed = true;
        }
    }
}
