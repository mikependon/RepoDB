using System;
using RepoDb.Attributes;
using RepoDb.Enumerations;

namespace RepoDb.IntegrationTests.Models
{
    [Map("[dbo].[Customer]")]
    public class Customer : DataEntity
    {
        [Attributes.Ignore(Command.Insert | Command.Update | Command.Merge | Command.InlineUpdate), Map("Id")]
        public int Id { get; set; }

        public Guid GlobalId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }

        public DateTime LastUpdatedUtc { get; set; }

        [Attributes.Ignore(Command.Update | Command.Insert)]
        public DateTime DateInsertedUtc { get; set; }

        public string LastUserId { get; set; }
    }
}