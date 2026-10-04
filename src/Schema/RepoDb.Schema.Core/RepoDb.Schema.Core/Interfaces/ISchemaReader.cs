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

namespace RepoDb.Schema
{
    /// <summary>
    /// An interface that is used to define the schema reader of the library. A schema reader reads the schema information
    /// (columns, keys, indexes and constraints) from a database so that the equivalent objects can be composed for another database.
    /// </summary>
    public interface ISchemaReader
    {
        #region Sync

        /// <summary>
        /// Gets the whole schema of the table (columns, primary key, indexes and constraints) in one call.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="TableSchema"/> of the table.</returns>
        TableSchema GetTableSchema(string tableName);

        /// <summary>
        /// Checks whether the table exists in the database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns><c>true</c> if the table exists; otherwise, <c>false</c>.</returns>
        bool TableExists(string tableName);

        /// <summary>
        /// Gets the name of the schema (i.e.: <c>dbo</c> or <c>public</c>) that owns the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The name of the owning schema, or <c>null</c> if the database has no schema concept.</returns>
        string GetSchemaName(string tableName);

        /// <summary>
        /// Gets the columns of the table, in their ordinal order.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="ColumnInfo"/> objects of the table.</returns>
        IEnumerable<ColumnInfo> GetColumns(string tableName);

        /// <summary>
        /// Gets the primary key of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="PrimaryKeyInfo"/> of the table, or <c>null</c> if the table has no primary key.</returns>
        PrimaryKeyInfo GetPrimaryKey(string tableName);

        /// <summary>
        /// Gets the indexes of the table. The primary key and the unique constraints are not included.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="IndexInfo"/> objects of the table.</returns>
        IEnumerable<IndexInfo> GetIndexes(string tableName);

        /// <summary>
        /// Gets the foreign keys of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="ForeignKeyInfo"/> objects of the table.</returns>
        IEnumerable<ForeignKeyInfo> GetForeignKeys(string tableName);

        /// <summary>
        /// Gets the unique constraints of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="UniqueConstraintInfo"/> objects of the table.</returns>
        IEnumerable<UniqueConstraintInfo> GetUniqueConstraints(string tableName);

        /// <summary>
        /// Gets the check constraints of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="CheckConstraintInfo"/> objects of the table.</returns>
        IEnumerable<CheckConstraintInfo> GetCheckConstraints(string tableName);

        /// <summary>
        /// Gets the names of the tables of the database.
        /// </summary>
        /// <param name="schemaName">The name of the schema to be read. The default is <c>null</c>, which reads all the schemas.</param>
        /// <returns>The names of the tables.</returns>
        IEnumerable<string> GetTables(string schemaName = null);

        /// <summary>
        /// Gets the relationships of the tables (as defined by their foreign keys), ordered so that a table always comes after the tables that it references.
        /// Use the order to create the tables, and the reverse of it to drop them. Only the relationships between the given tables are considered.
        /// If the tables reference each other in a circular way, no order exists for them, so the cycle is broken at the table that was given first.
        /// </summary>
        /// <param name="tableNames">The names of the tables.</param>
        /// <returns>The ordered <see cref="RelationshipInfo"/> objects, one per table.</returns>
        IEnumerable<RelationshipInfo> GetDependencyOrder(IEnumerable<string> tableNames);

        /// <summary>
        /// Gets the names of the given tables together with the names of the tables that are related to them, as defined by the foreign keys.
        /// Only the foreign keys are read (not the schema of the tables), so it is cheap to expand a table with its relationships.
        /// </summary>
        /// <param name="tableNames">The names of the tables.</param>
        /// <param name="relationshipBehavior">Defines which of the related tables are included. <see cref="CopySchemaRelationshipBehavior.TableOnly"/> returns only the given tables.</param>
        /// <returns>The names of the given tables (in the order that they were given), followed by the names of their related tables.</returns>
        IEnumerable<string> GetRelatedTables(IEnumerable<string> tableNames,
            CopySchemaRelationshipBehavior relationshipBehavior);

        #endregion

        #region Async

        /// <summary>
        /// Gets the whole schema of the table (columns, primary key, indexes and constraints) in one call.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="TableSchema"/> of the table.</returns>
        Task<TableSchema> GetTableSchemaAsync(string tableName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks whether the table exists in the database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: <c>true</c> if the table exists; otherwise, <c>false</c>.</returns>
        Task<bool> TableExistsAsync(string tableName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the name of the schema (i.e.: <c>dbo</c> or <c>public</c>) that owns the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the name of the owning schema, or <c>null</c> if the database has no schema concept.</returns>
        Task<string> GetSchemaNameAsync(string tableName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the columns of the table, in their ordinal order.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="ColumnInfo"/> objects of the table.</returns>
        Task<IEnumerable<ColumnInfo>> GetColumnsAsync(string tableName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the primary key of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="PrimaryKeyInfo"/> of the table, or <c>null</c> if the table has no primary key.</returns>
        Task<PrimaryKeyInfo> GetPrimaryKeyAsync(string tableName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the indexes of the table. The primary key and the unique constraints are not included.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="IndexInfo"/> objects of the table.</returns>
        Task<IEnumerable<IndexInfo>> GetIndexesAsync(string tableName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the foreign keys of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="ForeignKeyInfo"/> objects of the table.</returns>
        Task<IEnumerable<ForeignKeyInfo>> GetForeignKeysAsync(string tableName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the unique constraints of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="UniqueConstraintInfo"/> objects of the table.</returns>
        Task<IEnumerable<UniqueConstraintInfo>> GetUniqueConstraintsAsync(string tableName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the check constraints of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the <see cref="CheckConstraintInfo"/> objects of the table.</returns>
        Task<IEnumerable<CheckConstraintInfo>> GetCheckConstraintsAsync(string tableName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the names of the tables of the database.
        /// </summary>
        /// <param name="schemaName">The name of the schema to be read. The default is <c>null</c>, which reads all the schemas.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the names of the tables.</returns>
        Task<IEnumerable<string>> GetTablesAsync(string schemaName = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the relationships of the tables (as defined by their foreign keys), ordered so that a table always comes after the tables that it references.
        /// Use the order to create the tables, and the reverse of it to drop them. Only the relationships between the given tables are considered.
        /// If the tables reference each other in a circular way, no order exists for them, so the cycle is broken at the table that was given first.
        /// </summary>
        /// <param name="tableNames">The names of the tables.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the ordered <see cref="RelationshipInfo"/> objects, one per table.</returns>
        Task<IEnumerable<RelationshipInfo>> GetDependencyOrderAsync(IEnumerable<string> tableNames,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the names of the given tables together with the names of the tables that are related to them, as defined by the foreign keys.
        /// Only the foreign keys are read (not the schema of the tables), so it is cheap to expand a table with its relationships.
        /// </summary>
        /// <param name="tableNames">The names of the tables.</param>
        /// <param name="relationshipBehavior">Defines which of the related tables are included. <see cref="CopySchemaRelationshipBehavior.TableOnly"/> returns only the given tables.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains: the names of the given tables (in the order that they were given), followed by the names of their related tables.</returns>
        Task<IEnumerable<string>> GetRelatedTablesAsync(IEnumerable<string> tableNames,
            CopySchemaRelationshipBehavior relationshipBehavior,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
