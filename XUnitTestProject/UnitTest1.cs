using System;
using Xunit;
using BPServer.Core;
using BPServer.Core.Exceptions;

namespace XUnitTestProject
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var test = Assert.Throws<Exception>(() =>
              {
                  var temp = new RequestResponseMessage(new byte[] { 0x02, (byte)MessageType.RequestResponse, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
              });
        }
    }
}
