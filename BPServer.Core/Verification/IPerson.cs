using System;
using System.Collections.Generic;
using System.Text;

namespace BPServer.Core.Verification
{
    public interface IPerson
    {
        Guid Id { get; }
        ICollection<IRole> Roles { get; }
        ICollection<IIdentificator> Identificators { get; }
        string FirstName { get; }
        string LastName { get; }
        string FullName { get; }
        IContacts Contacts { get; }
        DateTime CreationDate { get; }
        DateTime LastModified { get; }

        void Load();
    }

    public interface IContacts 
    {
        string Email { get; }
        string Phone { get; }
    }

    public interface IRole 
    {
        Guid Id { get; }
        string Title { get; }
    }

    public interface IIdentificator
    {
        Guid Id { get; }
        ICollection<byte> Data { get; }
        string TypeName { get; }
    }

    public abstract class Identificator : IIdentificator
    {
        public Guid Id { get; protected set; }

        public ICollection<byte> Data { get; protected set; }

        public string TypeName { get; protected set; }

        protected Identificator(Guid id, ICollection<byte> data, string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new ArgumentException("typeName is empty", nameof(typeName));
            }
            if (id.Equals(Guid.Empty))
            {
                throw new ArgumentException("id is empty", nameof(id));
            }
            Id = id;
            Data = data ?? throw new ArgumentNullException(nameof(data));
            TypeName = typeName;
        }
    }

    public class CardIdentificator : Identificator
    {
        public CardIdentificator(Guid id, ICollection<byte> data) : base(id, data, typeof(CardIdentificator).Name)
        {
        }
    }

    public class BleIdentificator : Identificator
    {
        public BleIdentificator(Guid id, ICollection<byte> data) : base(id, data, typeof(BleIdentificator).Name)
        {
        }
    }

    public class FingerIdentificator : Identificator
    {
        public FingerIdentificator(Guid id, ICollection<byte> data) : base(id, data, typeof(FingerIdentificator).Name)
        {
        }
    }
}
