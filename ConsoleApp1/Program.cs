using BPServer.Core.Attributes;
using BPServer.Core.Handlers;
using BPServer.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPServer.Core.Extentions;

namespace ConsoleApp1
{
    class Program
    {

        static void Main(string[] args)
        {
            //var classTypesImplementingInterface = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
            //    .Where(mytype => typeof(IHandler).IsAssignableFrom(mytype) && mytype.GetInterfaces().Contains(typeof(IHandler)));
            //foreach (var item in classTypesImplementingInterface) 
            //{
            //    var temp = item.GetInterfaces();
            //    foreach(var type in temp)
            //    {
            //       // Console.WriteLine($"Type -- '{type.FullName}'");
            //        if (type.IsGenericType)
            //        {
            //            var tt = type.GenericTypeArguments;

            //            foreach (var ttt in tt)
            //            {
            //                Console.WriteLine($"Argument ---{ttt.FullName}");
            //                if (typeof(IMessage).IsAssignableFrom(ttt))
            //                {
            //                  //  DoSomething<item,ttt>();
            //                }
            //            }

            //        }

            //       // Console.WriteLine("____________________________________________");
            //    }


            //}
            var handler = new SomeFuckingHandler();
            var type = handler.GetType();
            var hashset = type.GetInterfaces().Where(x=>x.IsGenericType).SelectMany(x=>x.GenericTypeArguments).ToHashSet();  
            var commandTypes = hashset
                .Where(x => typeof(ICommand).IsAssignableFrom(x) && x.GetInterfaces().Contains(typeof(ICommand)))
                .ToDictionary(x => x.GetAttributeValue((CommandByteAttribute cbyte) => cbyte.Command));

            if (commandTypes.Count > 1)
            {
                throw new ArgumentException($"Handler of type: '{type}' has more than one ICommand");
            }
            var messageTypes = hashset
                .Where(x => typeof(IMessage).IsAssignableFrom(x) && x.GetInterfaces().Contains(typeof(IMessage)))
                .ToDictionary(x=>x.GetAttributeValue((MessageTypeAttribute mtype)=>mtype.MessageType));
          



            Console.WriteLine("MessageTypes:");
            foreach (var item in messageTypes)
            {
                Console.WriteLine($"Key: '{BitConverter.ToString(new byte[] { item.Key })}' Value: '{item.Value.Name}'");
            }
            Console.WriteLine("----------------");
            Console.WriteLine("CommandTypes:");
            foreach (var item in commandTypes)
            {
                Console.WriteLine($"Key: '{BitConverter.ToString(new byte[] { item.Key })}' Value: '{item.Value.Name}'");
            }
        }
    }

    [CommandByte(0xc7)]
    public class GetShit : ICommand
    {
        public byte Command => 0xc7;
    }

    [CommandByte(0xc8)]
    public class GetShit2 : ICommand
    {
        public byte Command => 0xc8;
    }

    public class SomeFuckingHandler : ICommandResponseHandler<GetShit>, ICommandResponseHandler<GetShit2>, IAcknowledgeHandler<GetShit>
    {
        public Task Handle(CommandResponseMessage input, IAddress address)
        {
            throw new NotImplementedException();
        }

        public Task Handle(AcknowledgeMessage input, IAddress address)
        {
            throw new NotImplementedException();
        }

        public Task Handle(NegativeAcknowledgeMessage input, IAddress address)
        {
            throw new NotImplementedException();
        }
    }
}
