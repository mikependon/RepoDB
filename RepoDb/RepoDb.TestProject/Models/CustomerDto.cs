using RepoDb.Attributes;
using System.Collections.Generic;

namespace RepoDb.TestProject.Models
{
    [Map("Customer")]
    public class CustomerDto : DataEntity
    {
        [Primary]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        //[Foreign("CustomerId")]
        public IEnumerable<OrderDto> Orders { get; set; }
    }
}
