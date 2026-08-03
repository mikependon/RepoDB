using NpgsqlTypes;
using RepoDb.Resolvers;
using System;

namespace RepoDb.PostgreSql.BulkOperations
{
    /// <summary>
    /// A class that is being used to define a mapping for the bulk insert operation for PostgeSQL.
    /// </summary>
    [Obsolete("This class is obsolete and will be removed in a future version. Use 'PostgreSqlBulkInsertMapItem' instead.")]
    public class NpgsqlBulkInsertMapItem : PostgreSqlBulkInsertMapItem
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
            base(sourceColumn, destinationColumn, (Type)null)
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
            base(sourceColumn, destinationColumn, type != null ? clientTypeToNpgsqlDbTypeResolver.Resolve(type) : null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="NpgsqlBulkInsertMapItem"/> object.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column or property. This respects the mapping of the properties if the source type is an entity model.</param>
        /// <param name="destinationColumn">The name of the destination column in the database.</param>
        /// <param name="dataTypeName">The PostgreSQL data type name (e.g. a custom enum type name) to be used when writing. Takes precedence over <see cref="NpgsqlDbType"/> when set.</param>
        public NpgsqlBulkInsertMapItem(string sourceColumn,
            string destinationColumn,
            string dataTypeName) :
            base(sourceColumn, destinationColumn, dataTypeName)
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
            base(sourceColumn, destinationColumn, npgsqlDbType)
        { }

        #endregion
    }
}
