using System;
using RepoDb.Enumerations;
using RepoDb.Attributes;

namespace RepoDb.TestProject
{
    [Map("[dbo].[Person]")]
    public class Person : DataEntity
    {
        [Ignore(Command.Insert | Command.Update | Command.Merge | Command.InlineUpdate)]
        [Primary(true)]
        [Map("Id")]
        public long Id { get; set; }

        [Map("Name")]
        public string Name { get; set; }

        public string Address { get; set; }

        public DateTime DateOfBirth { get; set; }

        [Map("Worth")]
        public decimal? Worth { get; set; }

        public Guid? SsId { get; set; }

        public DateTime DateUpdated { get; set; }

        [Ignore(Command.Update)]
        public DateTime DateInserted { get; set; }
    }
}
