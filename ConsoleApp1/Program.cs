using BPServer.Core.Handlers;
using BPServer.Core.Messages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        public static void DoSomething<T, TH>()
           where T : IMessage
           where TH : IHandler<IMessage>
        {
            Console.WriteLine("Subscribed!!!!!");
        }

        static void Main(string[] args)
        {
            var classTypesImplementingInterface = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(mytype => typeof(IHandler).IsAssignableFrom(mytype) && mytype.GetInterfaces().Contains(typeof(IHandler)));
            foreach (var item in classTypesImplementingInterface) 
            {
                var temp = item.GetInterfaces();
                foreach(var type in temp)
                {
                   // Console.WriteLine($"Type -- '{type.FullName}'");
                    if (type.IsGenericType)
                    {
                        var tt = type.GenericTypeArguments;
                     
                        foreach (var ttt in tt)
                        {
                            Console.WriteLine($"Argument ---{ttt.FullName}");
                            if (typeof(IMessage).IsAssignableFrom(ttt))
                            {
                              //  DoSomething<item,ttt>();
                            }
                        }
                       
                    }
                  
                   // Console.WriteLine("____________________________________________");
                }


            }
        }
    }

    public class GetShit : ICommand
    {
        public byte Command => throw new NotImplementedException();
    }

    public class SomeFuckingHandler : ICommandResponseHandler<GetShit>, IAcknowledgeHandler
    {
        public Task Handle(AcknowledgeMessage input)
        {
            throw new NotImplementedException();
        }

        public Task Handle(NegativeAcknowledgeMessage input)
        {
            throw new NotImplementedException();
        }

        public Task Handle(CommandResponseMessage input)
        {
            throw new NotImplementedException();
        }

        public byte Route()
        {
            throw new NotImplementedException();
        }
    }
}
