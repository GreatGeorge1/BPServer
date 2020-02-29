using System;
using System.ComponentModel.DataAnnotations;

namespace BPSever.Infrastracture.Entity
{
    public abstract class Entity<TId>
    {
        public TId Id { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime ModifiedTime { get; set; }
        public bool IsActive { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}

