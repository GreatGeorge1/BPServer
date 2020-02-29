using System;
using System.Collections.Generic;
using System.Text;

namespace BPServer.Core.Verification
{
    public interface IPersonService
    {
        IPerson FindByIdentificator(ICollection<byte> identificatorData);
        IPerson FindByIdentificator<TIdentificator>(ICollection<byte> identificatorData)
            where TIdentificator : IIdentificator;
    }
}
