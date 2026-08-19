using System;
using RepoDb.Connector.MariaDbConnector;

namespace RepoDb.MariaDbConnector.BulkOperations
{
    /// <summary>
    /// A class that is being used to define a mapping for the bulk operations against MariaDb.
    /// </summary>
    public class MariaDbBulkInsertMapItem : BulkInsertMapItem
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="MariaDbBulkInsertMapItem"/> object.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column or property. This respects the mapping of the properties if the source type is an entity model.</param>
        /// <param name="destinationColumn">The name of the destination column in the database.</param>
        public MariaDbBulkInsertMapItem(string sourceColumn,
            string destinationColumn) :
            this(sourceColumn, destinationColumn, null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="MariaDbBulkInsertMapItem"/> object.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column or property. This respects the mapping of the properties if the source type is an entity model.</param>
        /// <param name="destinationColumn">The name of the destination column in the database.</param>
        /// <param name="mariaDbType">
        /// The explicit <see cref="RepoDb.Connector.MariaDbConnector.MariaDbType"/> value to bind with for this column. When not
        /// provided, the type is inferred from the entity property's <c>[MariaDbType]</c> attribute
        /// (if present) or, failing that, from the .NET CLR value itself.
        /// </param>
        public MariaDbBulkInsertMapItem(string sourceColumn,
            string destinationColumn,
            MariaDbType? mariaDbType) :
            base(sourceColumn, destinationColumn)
        {
            MariaDbType = mariaDbType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the explicit <see cref="RepoDb.Connector.MariaDbConnector.MariaDbType"/> value to be used when writing.
        /// </summary>
        public MariaDbType? MariaDbType { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the string representation of the current object.
        /// </summary>
        /// <returns>The string representation of the current object.</returns>
        public override string ToString() =>
            $"{base.ToString()} ({MariaDbType})";

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
            hashCode = HashCode.Combine(hashCode, MariaDbType);

            return (this.hashCode = hashCode).Value;
        }

        #endregion
    }
}
