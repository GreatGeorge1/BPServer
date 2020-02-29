using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BPSever.Infrastracture.Entity
{

    public class Person : Entity<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [NotMapped]
        public string FullName { get { return FirstName + " " + LastName; } }
        public ICollection<Identificator> Identificators { get; set; }
        public string ExternalId { get; set; }
    }

    public abstract class Identificator : Entity<int>
    {
        public byte[] Data { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }
    }

    public class CardIdentificator : Identificator
    {

    }

    public class FingerIdentificator : Identificator
    {

    }

    public class BleIdentificator : Identificator
    {

    }
}

