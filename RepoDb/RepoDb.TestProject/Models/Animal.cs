using RepoDb.Attributes;
using System;
using System.Data;

namespace RepoDb.TestProject.Models
{
    [Map("[dbo].[Animal]")]
    public class Animal : DataEntity
    {
        public Guid Id { get; set; }

        [TypeMap(DbType.String)]
        public string Name { get; set; }

        public string Address { get; set; }

        public DateTime DateUpdated { get; set; }

        public DateTime DateInserted { get; set; }
    }
}
