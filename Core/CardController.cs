namespace BPServer.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using BPServer.Core.Attributes;

    [Route(0xC7)]
    public class CardController : INotificationController
    {
        public Task Notification(NotificationMessage input)
        {
            throw new NotImplementedException();
        }
    }
}
