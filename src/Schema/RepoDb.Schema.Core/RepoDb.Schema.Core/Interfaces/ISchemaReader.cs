#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using System.Data;

namespace RepoDb.Schema
{
    /// <summary>
    /// An interface that is used to define the schema reader of the library. A schema reader reads the schema information
    /// (columns, keys, indexes and constraints) from a database so that the equivalent objects can be composed for another database.
    /// </summary>
    public interface ISchemaReader
    {
        #region Public Methods

        /// <summary>
        /// Gets the whole schema of the table (columns, primary key, indexes and constraints) in one call.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The <see cref="TableSchema"/> of the table.</returns>
        TableSchema GetTableSchema(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null);

        /// <summary>
        /// Checks whether the table exists in the database.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns><c>true</c> if the table exists; otherwise, <c>false</c>.</returns>
        bool TableExists(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null);

        /// <summary>
        /// Gets the name of the schema (i.e.: <c>dbo</c> or <c>public</c>) that owns the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The name of the owning schema, or <c>null</c> if the database has no schema concept.</returns>
        string GetSchemaName(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null);

        /// <summary>
        /// Gets the columns of the table, in their ordinal order.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The <see cref="ColumnInfo"/> objects of the table.</returns>
        IEnumerable<ColumnInfo> GetColumns(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null);

        /// <summary>
        /// Gets the primary key of the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The <see cref="PrimaryKeyInfo"/> of the table, or <c>null</c> if the table has no primary key.</returns>
        PrimaryKeyInfo GetPrimaryKey(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null);

        /// <summary>
        /// Gets the indexes of the table. The primary key and the unique constraints are not included.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The <see cref="IndexInfo"/> objects of the table.</returns>
        IEnumerable<IndexInfo> GetIndexes(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null);

        /// <summary>
        /// Gets the foreign keys of the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The <see cref="ForeignKeyInfo"/> objects of the table.</returns>
        IEnumerable<ForeignKeyInfo> GetForeignKeys(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null);

        /// <summary>
        /// Gets the unique constraints of the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The <see cref="UniqueConstraintInfo"/> objects of the table.</returns>
        IEnumerable<UniqueConstraintInfo> GetUniqueConstraints(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null);

        /// <summary>
        /// Gets the check constraints of the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The <see cref="CheckConstraintInfo"/> objects of the table.</returns>
        IEnumerable<CheckConstraintInfo> GetCheckConstraints(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null);

        /// <summary>
        /// Gets the schemas of the database (the system schemas are not included). A provider where the schema is a database or a user (i.e.: a database or a user) returns them.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The <see cref="SchemaInfo"/> objects of the database, or an empty list if the database has no schema concept.</returns>
        IEnumerable<SchemaInfo> GetSchemas(IDbConnection connection,
            IDbTransaction transaction = null);

        /// <summary>
        /// Gets the names of the tables of the database.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="schemaName">The name of the schema to be read. The default is <c>null</c>, which reads all the schemas.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The names of the tables.</returns>
        IEnumerable<string> GetTables(IDbConnection connection,
            string schemaName = null,
            IDbTransaction transaction = null);

        /// <summary>
        /// Gets the relationships of the tables (as defined by their foreign keys), ordered so that a table always comes after the tables that it references.
        /// Use the order to create the tables, and the reverse of it to drop them. Only the relationships between the given tables are considered.
        /// If the tables reference each other in a circular way, no order exists for them, so the cycle is broken at the table that was given first.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableNames">The names of the tables.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The ordered <see cref="RelationshipInfo"/> objects, one per table.</returns>
        IEnumerable<RelationshipInfo> GetDependencyOrder(IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbTransaction transaction = null);

        /// <summary>
        /// Gets the names of the given tables together with the names of the tables that are related to them, as defined by the foreign keys.
        /// Only the foreign keys are read (not the schema of the tables), so it is cheap to expand a table with its relationships.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableNames">The names of the tables.</param>
        /// <param name="relationshipBehavior">Defines which of the related tables are included. <see cref="RelatedTableBehavior.TableOnly"/> returns only the given tables.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <returns>The names of the given tables (in the order that they were given), followed by the names of their related tables.</returns>
        IEnumerable<string> GetRelatedTables(IDbConnection connection,
            IEnumerable<string> tableNames,
            RelatedTableBehavior relationshipBehavior,
            IDbTransaction transaction = null);

        /// <summary>
        /// Gets the whole schema of the table (columns, primary key, indexes and constraints) in one call.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="TableSchema"/> of the table.</returns>
        Task<TableSchema> GetTableSchemaAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks whether the table exists in the database.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: <c>true</c> if the table exists; otherwise, <c>false</c>.</returns>
        Task<bool> TableExistsAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the name of the schema (i.e.: <c>dbo</c> or <c>public</c>) that owns the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the name of the owning schema, or <c>null</c> if the database has no schema concept.</returns>
        Task<string> GetSchemaNameAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the columns of the table, in their ordinal order.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="ColumnInfo"/> objects of the table.</returns>
        Task<IEnumerable<ColumnInfo>> GetColumnsAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the primary key of the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="PrimaryKeyInfo"/> of the table, or <c>null</c> if the table has no primary key.</returns>
        Task<PrimaryKeyInfo> GetPrimaryKeyAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the indexes of the table. The primary key and the unique constraints are not included.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="IndexInfo"/> objects of the table.</returns>
        Task<IEnumerable<IndexInfo>> GetIndexesAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the foreign keys of the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="ForeignKeyInfo"/> objects of the table.</returns>
        Task<IEnumerable<ForeignKeyInfo>> GetForeignKeysAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the unique constraints of the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="UniqueConstraintInfo"/> objects of the table.</returns>
        Task<IEnumerable<UniqueConstraintInfo>> GetUniqueConstraintsAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the check constraints of the table.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="CheckConstraintInfo"/> objects of the table.</returns>
        Task<IEnumerable<CheckConstraintInfo>> GetCheckConstraintsAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the schemas of the database (the system schemas are not included). A provider where the schema is a database or a user (i.e.: a database or a user) returns them.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="SchemaInfo"/> objects of the database, or an empty list if the database has no schema concept.</returns>
        Task<IEnumerable<SchemaInfo>> GetSchemasAsync(IDbConnection connection,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the names of the tables of the database.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="schemaName">The name of the schema to be read. The default is <c>null</c>, which reads all the schemas.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the names of the tables.</returns>
        Task<IEnumerable<string>> GetTablesAsync(IDbConnection connection,
            string schemaName = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the relationships of the tables (as defined by their foreign keys), ordered so that a table always comes after the tables that it references.
        /// Use the order to create the tables, and the reverse of it to drop them. Only the relationships between the given tables are considered.
        /// If the tables reference each other in a circular way, no order exists for them, so the cycle is broken at the table that was given first.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableNames">The names of the tables.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the ordered <see cref="RelationshipInfo"/> objects, one per table.</returns>
        Task<IEnumerable<RelationshipInfo>> GetDependencyOrderAsync(IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the names of the given tables together with the names of the tables that are related to them, as defined by the foreign keys.
        /// Only the foreign keys are read (not the schema of the tables), so it is cheap to expand a table with its relationships.
        /// </summary>
        /// <param name="connection">The operation connection.</param>
        /// <param name="tableNames">The names of the tables.</param>
        /// <param name="relationshipBehavior">Defines which of the related tables are included. <see cref="RelatedTableBehavior.TableOnly"/> returns only the given tables.</param>
        /// <param name="transaction">The operation transaction.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the names of the given tables (in the order that they were given), followed by the names of their related tables.</returns>
        Task<IEnumerable<string>> GetRelatedTablesAsync(IDbConnection connection,
            IEnumerable<string> tableNames,
            RelatedTableBehavior relationshipBehavior,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default);
        #endregion
    }
}
