using RepoDb;
using RepoDb.Attributes;

namespace RepoDb.TestProject
{
    [Map("Customer")]
    public class CustomerDto : DataEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
    }
}
