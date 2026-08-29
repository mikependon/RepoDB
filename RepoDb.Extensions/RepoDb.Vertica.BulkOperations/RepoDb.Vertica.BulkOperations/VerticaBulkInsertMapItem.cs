using System;
using Vertica.Data.VerticaClient;

namespace RepoDb.Vertica.BulkOperations
{
    /// <summary>
    /// A class that is being used to define a mapping for the bulk operations against Vertica.
    /// </summary>
    public class VerticaBulkInsertMapItem : BulkInsertMapItem
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="VerticaBulkInsertMapItem"/> object.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column or property. This respects the mapping of the properties if the source type is an entity model.</param>
        /// <param name="destinationColumn">The name of the destination column in the database.</param>
        public VerticaBulkInsertMapItem(string sourceColumn,
            string destinationColumn) :
            this(sourceColumn, destinationColumn, null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="VerticaBulkInsertMapItem"/> object.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column or property. This respects the mapping of the properties if the source type is an entity model.</param>
        /// <param name="destinationColumn">The name of the destination column in the database.</param>
        /// <param name="type">
        /// The explicit <see cref="Vertica.Data.VerticaClient.VerticaType"/> value to bind with for this column
        /// (named <c>type</c>, matching <see cref="Vertica.Data.VerticaClient.VerticaParameter.Type"/>'s own name).
        /// Not consumed by the current COPY-stream-based bulk-copy implementation - Vertica's COPY parser
        /// infers each field's wire format from the destination column's actual server-side type - so this
        /// is kept only as a forward-looking escape hatch, matching the equivalent parameter on every other
        /// bulk-operations package's map-item type.
        /// </param>
        public VerticaBulkInsertMapItem(string sourceColumn,
            string destinationColumn,
            VerticaType? type) :
            base(sourceColumn, destinationColumn)
        {
            Type = type;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the explicit <see cref="Vertica.Data.VerticaClient.VerticaType"/> value to be used when writing.
        /// </summary>
        public VerticaType? Type { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the string representation of the current object.
        /// </summary>
        /// <returns>The string representation of the current object.</returns>
        public override string ToString() =>
            $"{base.ToString()} ({Type})";

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
            hashCode = HashCode.Combine(hashCode, Type);

            return (this.hashCode = hashCode).Value;
        }

        #endregion
    }
}
