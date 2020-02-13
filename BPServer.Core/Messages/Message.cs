namespace BPServer.Core.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BPServer.Core.Attributes;
    using Dawn;

    [MessageType((byte)MessageType.NotSet)]
    public abstract class Message : IMessage
    {
        public byte[] Raw { get; private set; }

        public ICollection<byte> Body { get; private set; }

        public byte Command { get; private set; }

        public byte Type { get; protected set; }

        public byte BodyXor { get; private set; }


        public Guid Id { get; private set; }
        public DateTime CreationDate {get; private set;}

        protected Message(Guid id,byte[] message)
        {
            Id = id;
            CreationDate = DateTime.UtcNow;

            Raw = Guard.Argument(message, nameof(message)).NotNull().MinCount(8);
            Guard.Argument(message[0]==0x02).True();
            Command = message[2];
            //var flag = TypeOf(message, out MessageType type);
            Type = message[1];
            Guard.Argument(IsValidCheckSum(Raw)).True();
            BodyXor = message[3];
            var length = HighLowToInt(message.ElementAt(4), message.ElementAt(5));
            Guard.Argument<bool>(message.Length - 6 == length).True();
            Body = message.Skip(6).Take(length).ToArray();
        }

        protected Message(byte[] message): this(Guid.NewGuid(), message)
        {
        }

        protected Message(byte Mtype, byte Route, byte[] Value)
            : this(Guid.NewGuid(),Mtype, Route, Value) 
        {
        }

        protected Message(Guid id,byte Mtype,byte Route, byte[] Value) 
        {
            Id = id;
            CreationDate = DateTime.UtcNow;

            Guard.Argument(Value, nameof(Value)).NotNull().MinCount(2);
            //Guard.Argument(Mtype).Defined();
            var xor = CalCheckSum(Value);
            var raw = new List<byte>(){
                0x02,
                (byte)Mtype,
                Route,
                xor
            };
            raw.AddRange(IntToHighLow(Value.Length));
            foreach (var item in Value)
            {
                raw.Add(item);
            }
            Raw = raw.ToArray();
            Type = Mtype;
            this.Command = Route;
            BodyXor = xor;
            Body = Value;
        }

        public static bool IsValid(byte[] value)
        {
            if (value.Length < 8) return false;
            if (value[0] != 0x02) return false;
            var lengthBytes = new byte[] { value[4], value[5] };
            var length = HighLowToInt(value[4], value[5]);
            var body = value.Skip(6).ToArray();
            if (body.Length != length) return false;
            var checksum = value[3];
            var newChecksum = CalCheckSum(body);
            if (checksum != newChecksum) return false;
            return true;
        }

        protected static bool IsTypeOf(byte[] message, MessageType type)
        {
            Guard.Argument(message, nameof(message)).NotNull().MinCount(8);
            var temp = message[1];
            if (temp == Convert.ToInt32(type))
            {
                return true;
            }
            return false;
        }

        private static bool IsValidCheckSum(byte[] message)
        {
            var temp = HighLowToInt(message[4], message[5]);
            var bytes = message.Skip(6).Take(temp).ToArray();
            var checksum = message[3];
            var newChecksum = CalCheckSum(bytes);
            if (checksum == newChecksum)
            {
                return true;
            }
            return false;
        }

        public static byte CalCheckSum(byte[] PacketData)
        {
            Byte _CheckSumByte = 0x00;
            for (int i = 0; i < PacketData.Length; i++)
                _CheckSumByte ^= PacketData[i];

            return _CheckSumByte;
        }

        /// <summary>
        /// if false, no such type in MessageType
        /// </summary>
        /// <param name="message"></param>
        /// <param name="mtype"></param>
        /// <returns></returns>
        public static bool TypeOf(byte[] message, out MessageType mtype)
        {
            var temp = message[1];
            mtype = MessageType.NotSet;
            foreach (var item in Enum.GetValues(typeof(MessageType)))
            {
                if (temp == Convert.ToInt32(item))
                {
                    mtype = (MessageType)item;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// byte[] length 2
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] IntToHighLow(int input)
        {
            int test = input;
            byte low = (byte)(test & 0xff);
            byte high = (byte)((test >> 8) & 0xff);
            // int res = low | (high << 8);
            return new byte[] { high, low };
        }

        public static int HighLowToInt(byte high, byte low)
        {
            int res = low | (high << 8);
            return res;
        }
    }
}
