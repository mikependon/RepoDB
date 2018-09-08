using RepoDb;
using System;

namespace Project_ORM_Perf_Benchmark
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int? Age { get; set; }
        public float? Worth { get; set; }
        public double? Salary { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public short Gender { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
