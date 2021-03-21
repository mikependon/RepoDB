using System;
using Dapper.Contrib.Extensions;

namespace RepoDb.Benchmarks.PostgreSql.Models
{
    [Table("\"Person\"")]
    public class Person
    {
        [Key]
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int Age { get; set; }
        public virtual DateTime CreatedDateUtc { get; set; }
    }
}