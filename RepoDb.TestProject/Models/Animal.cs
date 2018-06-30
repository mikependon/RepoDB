using RepoDb.Attributes;
using System;

namespace RepoDb.TestProject.Models
{
    [Map("[dbo].[Animal]")]
    public class Animal : DataEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public DateTime DateUpdated { get; set; }

        public DateTime DateInserted { get; set; }
    }
}
