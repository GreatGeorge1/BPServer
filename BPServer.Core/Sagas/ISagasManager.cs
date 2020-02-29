using BPServer.Core.MessageBus.Handlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPServer.Core.Sagas
{
    public interface ISagasManager 
    {
        void AddSaga(ISaga serverSaga);
        bool TryGet(Guid id, out ISaga saga);
        bool TryGet(string transportName, ICommand command, out ISaga saga);
        bool HasSagas(string transportName);
        bool TryGetSagas(string transportName, out ICollection<ISaga> sagas);
    }
}
