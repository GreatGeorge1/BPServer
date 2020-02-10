using BPServer.Core.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BPServer.Core.Sagas
{
    public class SagasManager : ISagasManager
    {
        protected readonly List<ISaga> _sagas;
        public SagasManager()
        {
            _sagas = new List<ISaga>();;
        }

        protected void OnCompleted(object sender, Guid id)
        {
            var saga = _sagas.First(x => x.Id == id);
            _sagas.Remove(saga);//TODO save
            Console.WriteLine($"Saga Completed: '{saga.Id}', removed");
        }

        protected void OnError(object sender, Guid id)
        {
            Console.WriteLine($"Saga Error: '{id}'");
            var saga = _sagas.First(x => x.Id == id);
            saga.RepeatIncrement();
        }

        protected void OnTimeoutReached(object sender, Guid id)
        {
            Console.WriteLine($"Saga Timeout: '{id}'");
            var saga = _sagas.First(x => x.Id == id);
            if (saga.IsCompleted)
            {
                return;
            }
            saga.RepeatIncrement();
        }

        protected void OnRepeatLimitReached(object sender, Guid id)
        {
            Console.WriteLine($"Saga RepeatLimitReached: '{id}', removed");
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
                .Where(x => x.SerialPort.Equals(serialPort))
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
    }
}
