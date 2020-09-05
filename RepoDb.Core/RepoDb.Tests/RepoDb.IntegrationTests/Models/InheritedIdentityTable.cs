using RepoDb.Attributes;
using System;

namespace RepoDb.IntegrationTests.Models
{
    public abstract class Entity<T> : IEquatable<T>
        where T : class
    {
        public long Id { get; set; }
        public Guid RowGuid { get; set; }
        public bool? ColumnBit { get; set; }
        public DateTime? ColumnDateTime { get; set; }
        public DateTime? ColumnDateTime2 { get; set; }

        public bool Equals(T other)
        {
            if (other is null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            if (GetType() != other.GetType())
            {
                return false;
            }
            return false;
        }
    }

    [Map("[sc].[IdentityTable]")]
    public class InheritedIdentityTable : Entity<InheritedIdentityTable>
    {
        public decimal? ColumnDecimal { get; set; }
        public double? ColumnFloat { get; set; }
        public int? ColumnInt { get; set; }
        public string ColumnNVarChar { get; set; }
    }
}
