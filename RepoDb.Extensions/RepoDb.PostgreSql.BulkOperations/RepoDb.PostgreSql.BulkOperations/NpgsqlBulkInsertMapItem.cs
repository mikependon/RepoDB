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

        /// <summary>
        /// Creates a new instance of <see cref="BulkInsertMapItem"/> object.
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
            this(sourceColumn, destinationColumn, clientTypeToNpgsqlDbTypeResolver.Resolve(type))
        { }

        /// <summary>
        /// Creates a new instance of <see cref="BulkInsertMapItem"/> object.
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

        /// <summary>
        /// Gets the <see cref="NpgsqlTypes.NpgsqlDbType"/> type value being used when writing.
        /// </summary>
        public NpgsqlDbType? NpgsqlDbType { get; }
    }
}
