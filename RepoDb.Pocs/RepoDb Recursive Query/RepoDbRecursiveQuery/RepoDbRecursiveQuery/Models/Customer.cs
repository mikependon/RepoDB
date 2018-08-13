using RepoDb;
using System.Collections.Generic;

namespace RepoDbRecursiveQuery.Models
{
    public class Customer : DataEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }
}
