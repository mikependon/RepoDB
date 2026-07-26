using Oracle.ManagedDataAccess.Client;
using System;

namespace RepoDb.Oracle.BulkOperations
{
    /// <summary>
    /// A class that is being used to define a mapping for the bulk operations against Oracle.
    /// </summary>
    public class OracleBulkInsertMapItem : BulkInsertMapItem
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="OracleBulkInsertMapItem"/> object.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column or property. This respects the mapping of the properties if the source type is an entity model.</param>
        /// <param name="destinationColumn">The name of the destination column in the database.</param>
        public OracleBulkInsertMapItem(string sourceColumn,
            string destinationColumn) :
            this(sourceColumn, destinationColumn, (OracleDbType?)null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="OracleBulkInsertMapItem"/> object.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column or property. This respects the mapping of the properties if the source type is an entity model.</param>
        /// <param name="destinationColumn">The name of the destination column in the database.</param>
        /// <param name="oracleDbType">
        /// The explicit <see cref="Oracle.ManagedDataAccess.Client.OracleDbType"/> value to bind with for this column. When not
        /// provided, the type is inferred from the entity property's <c>[OracleDbType]</c>/<c>[OracleDbTypeEx]</c> attribute
        /// (if present) or, failing that, from the .NET CLR value itself.
        /// </param>
        public OracleBulkInsertMapItem(string sourceColumn,
            string destinationColumn,
            OracleDbType? oracleDbType) :
            base(sourceColumn, destinationColumn)
        {
            OracleDbType = oracleDbType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the explicit <see cref="Oracle.ManagedDataAccess.Client.OracleDbType"/> value to be used when writing.
        /// </summary>
        public OracleDbType? OracleDbType { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the string representation of the current object.
        /// </summary>
        /// <returns>The string representation of the current object.</returns>
        public override string ToString() =>
            $"{base.ToString()} ({OracleDbType})";

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
            hashCode = HashCode.Combine(hashCode, OracleDbType);

            return (this.hashCode = hashCode).Value;
        }

        #endregion
    }
}
