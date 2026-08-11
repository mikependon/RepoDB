using System;
using IBM.Data.Db2;

namespace RepoDb.Db2.BulkOperations
{
    /// <summary>
    /// A class that is being used to define a mapping for the bulk operations against Db2.
    /// </summary>
    public class Db2BulkInsertMapItem : BulkInsertMapItem
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="Db2BulkInsertMapItem"/> object.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column or property. This respects the mapping of the properties if the source type is an entity model.</param>
        /// <param name="destinationColumn">The name of the destination column in the database.</param>
        public Db2BulkInsertMapItem(string sourceColumn,
            string destinationColumn) :
            this(sourceColumn, destinationColumn, null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="Db2BulkInsertMapItem"/> object.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column or property. This respects the mapping of the properties if the source type is an entity model.</param>
        /// <param name="destinationColumn">The name of the destination column in the database.</param>
        /// <param name="db2Type">
        /// The explicit <see cref="Db2.DB2Type"/> value to bind with for this column. When not
        /// provided, the type is inferred from the entity property's <c>[DB2Type]</c>/<c>[Db2DbTypeEx]</c> attribute
        /// (if present) or, failing that, from the .NET CLR value itself.
        /// </param>
        public Db2BulkInsertMapItem(string sourceColumn,
            string destinationColumn,
            DB2Type? db2Type) :
            base(sourceColumn, destinationColumn)
        {
            DB2Type = db2Type;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the explicit <see cref="Db2.ManagedDataAccess.Client.DB2Type"/> value to be used when writing.
        /// </summary>
        public DB2Type? DB2Type { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the string representation of the current object.
        /// </summary>
        /// <returns>The string representation of the current object.</returns>
        public override string ToString() =>
            $"{base.ToString()} ({DB2Type})";

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
            hashCode = HashCode.Combine(hashCode, DB2Type);

            return (this.hashCode = hashCode).Value;
        }

        #endregion
    }
}
