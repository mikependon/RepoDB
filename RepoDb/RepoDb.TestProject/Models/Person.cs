using System;

namespace RepoDb.TestProject.Models
{
    public class Person : DataEntity
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public DateTime DateOfBirth { get; set; }

        public decimal? Worth { get; set; }

        public Guid? SsId { get; set; }

        public DateTime DateUpdated { get; set; }

        public DateTime DateInserted { get; set; }
    }
}
