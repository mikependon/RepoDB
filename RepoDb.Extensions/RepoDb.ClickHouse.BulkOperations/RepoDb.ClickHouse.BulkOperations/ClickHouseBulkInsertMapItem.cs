using System;

namespace RepoDb.ClickHouse.BulkOperations
{
    /// <summary>
    /// A class that is being used to define a mapping for the bulk operations against ClickHouse.
    /// </summary>
    public class ClickHouseBulkInsertMapItem : BulkInsertMapItem
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="ClickHouseBulkInsertMapItem"/> object.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column or property. This respects the mapping of the properties if the source type is an entity model.</param>
        /// <param name="destinationColumn">The name of the destination column in the database.</param>
        public ClickHouseBulkInsertMapItem(string sourceColumn,
            string destinationColumn) :
            this(sourceColumn, destinationColumn, null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="ClickHouseBulkInsertMapItem"/> object.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column or property. This respects the mapping of the properties if the source type is an entity model.</param>
        /// <param name="destinationColumn">The name of the destination column in the database.</param>
        /// <param name="clickHouseType">
        /// The explicit ClickHouse type name (e.g. <c>"UInt64"</c>, <c>"Nullable(String)"</c>) to bind with for
        /// this column. When not provided, the type is inferred from the entity property's
        /// <c>[RepoDb.Attributes.Parameter.ClickHouse.ClickHouseTypeAttribute]</c> (if present) or, failing
        /// that, from the .NET CLR value itself - mirroring the convention already established by
        /// <c>RepoDb.ClickHouse</c>'s own <c>ClickHouseTypeAttribute</c>.
        /// </param>
        public ClickHouseBulkInsertMapItem(string sourceColumn,
            string destinationColumn,
            string clickHouseType) :
            base(sourceColumn, destinationColumn)
        {
            ClickHouseType = clickHouseType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the explicit ClickHouse type name to be used when writing (e.g. <c>"UInt64"</c>,
        /// <c>"Nullable(String)"</c>).
        /// </summary>
        public string ClickHouseType { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the string representation of the current object.
        /// </summary>
        /// <returns>The string representation of the current object.</returns>
        public override string ToString() =>
            $"{base.ToString()} ({ClickHouseType})";

        #endregion

        #region Equality and comparers

        private int? hashCode = null;

        /// <summary>
        /// Returns the hashcode of the current instance.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            if (this.hashCode != null)
            {
                return this.hashCode.Value;
            }

            var hashCode = base.GetHashCode();
            hashCode = HashCode.Combine(hashCode, ClickHouseType);

            return (this.hashCode = hashCode).Value;
        }

        #endregion
    }
}
