using RepoDb.Attributes;
using System;

namespace RepoDb.IntegrationTests.Models
{
    public class NonKeyedTable
    {
        public int? ColumnInt { get; set; }
        public string ColumnNVarChar { get; set; }
        public DateTime? ColumnDateTime2 { get; set; }
    }
}
