using System;
using System.Collections.Generic;
using System.Text;

namespace BPSever.Infrastracture.MessageTypes
{
    public interface TestEvent
    {
        string Text { get; }
    }

    public interface TestRequest
    {
        string Text { get; }
        string Id { get; }
    }

    public interface TestRequestResult
    {
        string Result { get; }
        string Id { get; }
    }
}
