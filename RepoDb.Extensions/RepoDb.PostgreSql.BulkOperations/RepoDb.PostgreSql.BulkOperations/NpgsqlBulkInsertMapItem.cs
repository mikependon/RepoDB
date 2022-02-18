using NpgsqlTypes;
using RepoDb.Resolvers;
using System;

namespace RepoDb.PostgreSql.BulkOperations
{
    /// <summary>
    /// A class that is being used to define a mapping for the bulk insert operation for PostgeSQL.
    /// </summary>
    public class NpgsqlBulkInsertMapItem : BulkInsertMapItem
    {
        private static readonly ClientTypeToNpgsqlDbTypeResolver clientTypeToNpgsqlDbTypeResolver = new ClientTypeToNpgsqlDbTypeResolver();

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="NpgsqlBulkInsertMapItem"/> object.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column or property. This respects the mapping of the properties if the source type is an entity model.</param>
        /// <param name="destinationColumn">The name of the destination column in the database.</param>
        public NpgsqlBulkInsertMapItem(string sourceColumn,
            string destinationColumn) :
            this(sourceColumn, destinationColumn, (Type)null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="NpgsqlBulkInsertMapItem"/> object.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column or property. This respects the mapping of the properties if the source type is an entity model.</param>
        /// <param name="destinationColumn">The name of the destination column in the database.</param>
        /// <param name="type">
        /// The .NET CLR type to be used to identify the equivalent <see cref="NpgsqlTypes.NpgsqlDbType"/> value. The <see cref="ClientTypeToNpgsqlDbTypeResolver"/> object
        /// is used for identification.
        /// </param>
        public NpgsqlBulkInsertMapItem(string sourceColumn,
            string destinationColumn,
            Type type) :
            this(sourceColumn, destinationColumn, type != null ? clientTypeToNpgsqlDbTypeResolver.Resolve(type) : null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="NpgsqlBulkInsertMapItem"/> object.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column or property. This respects the mapping of the properties if the source type is an entity model.</param>
        /// <param name="destinationColumn">The name of the destination column in the database.</param>
        /// <param name="npgsqlDbType">The <see cref="NpgsqlTypes.NpgsqlDbType"/> value to be used when writing.</param>
        public NpgsqlBulkInsertMapItem(string sourceColumn,
            string destinationColumn,
            NpgsqlDbType? npgsqlDbType) :
            base(sourceColumn, destinationColumn)
        {
            NpgsqlDbType = npgsqlDbType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="NpgsqlTypes.NpgsqlDbType"/> type value being used when writing.
        /// </summary>
        public NpgsqlDbType? NpgsqlDbType { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the string representation of the current object.
        /// </summary>
        /// <returns>The string representation of the current object.</returns>
        public override string ToString() =>
            $"{base.ToString()} ({NpgsqlDbType})";

        #endregion

        #region Equality and comparers

        private int? hashCode = null;

        /// <summary>
        /// Returns the hashcode of the current instance.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            // Make sure to return if it is already provided
            if (this.hashCode != null)
            {
                return this.hashCode.Value;
            }

            // Base
            var hashCode = base.GetHashCode();

            // NpgsqlDbType
            hashCode = HashCode.Combine(hashCode, NpgsqlDbType);

            // Set and return the hashcode
            return (this.hashCode = hashCode).Value;
        }

        #endregion
    }
}
