using BPServer.Core.Handlers;
using System;
using System.Text;

namespace BPServer.Core.Sagas
{
    public interface ISagasManager 
    {
        void AddSaga(ISaga serverSaga);
        bool TryGet(Guid id, out ISaga saga);
        bool TryGet(string serialPort, ICommand command, out ISaga saga);
    }
}
