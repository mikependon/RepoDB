using System;

namespace RepoDb.Benchmarks.Models
{
    public class Person
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int Age { get; set; }
        public virtual DateTime CreatedDateUtc { get; set; }
    }
}