#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.MariaDb.IntegrationTests.Setup
{
    /// <summary>
    /// A composer that does the same as the composer that it wraps, but does not compose the statements that check whether a table, a column or an index exists.
    /// A copy that uses it does not know what exists in the destination database, so it always tries to create the tables (it is used to make a statement fail).
    /// </summary>
    public sealed class NoExistenceCheckSchemaComposer : ISchemaComposer
    {
        #region Private Variables

        private readonly ISchemaComposer _composer;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="NoExistenceCheckSchemaComposer"/> class.
        /// </summary>
        /// <param name="composer">The composer to be wrapped.</param>
        public NoExistenceCheckSchemaComposer(ISchemaComposer composer)
        {
            _composer = composer;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Composes the whole script that creates the table, its indexes and its foreign keys, as an ordered list of statements.
        /// </summary>
        /// <param name="schema">The schema of the table.</param>
        /// <returns>The ordered SQL statements. Execute them in order.</returns>
        public IEnumerable<string> ComposeSchema(TableSchema schema) => _composer.ComposeSchema(schema);

        /// <summary>
        /// Composes the whole script that creates the tables, their indexes and their foreign keys, as an ordered list of statements.
        /// </summary>
        /// <param name="schemas">The schemas of the tables.</param>
        /// <returns>The ordered SQL statements. Execute them in order.</returns>
        public IEnumerable<string> ComposeSchemas(IEnumerable<TableSchema> schemas) => _composer.ComposeSchemas(schemas);

        /// <summary>
        /// Composes the statement that creates the table, including its columns, primary key, unique constraints and check constraints.
        /// </summary>
        /// <param name="schema">The schema of the table.</param>
        /// <returns>The SQL statement.</returns>
        public string ComposeCreateTable(TableSchema schema) => _composer.ComposeCreateTable(schema);

        /// <summary>
        /// Composes the statement that creates an index.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="index">The index to be created.</param>
        /// <returns>The SQL statement.</returns>
        public string ComposeCreateIndex(string tableName, IndexInfo index) => _composer.ComposeCreateIndex(tableName, index);

        /// <summary>
        /// Composes the statement that adds a foreign key to a table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="foreignKey">The foreign key to be added.</param>
        /// <returns>The SQL statement.</returns>
        public string ComposeAddForeignKey(string tableName, ForeignKeyInfo foreignKey) => _composer.ComposeAddForeignKey(tableName, foreignKey);

        /// <summary>
        /// Composes the statement that adds a column to a table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="column">The column to be added.</param>
        /// <returns>The SQL statement.</returns>
        public string ComposeAddColumn(string tableName, ColumnInfo column) => _composer.ComposeAddColumn(tableName, column);

        /// <summary>
        /// Composes the statement that drops a table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The SQL statement.</returns>
        public string ComposeDropTable(string tableName) => _composer.ComposeDropTable(tableName);

        /// <summary>
        /// Composes the name of the table, in the dialect of the destination database.
        /// </summary>
        /// <param name="table">The identity (name and schema) of the table.</param>
        /// <returns>The name of the table.</returns>
        public string ComposeName(TableInfo table) => _composer.ComposeName(table);

        /// <summary>
        /// Does not compose the statement that checks whether the table exists, so the existence of the table is not known.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns><c>null</c>.</returns>
        public string ComposeTableExists(string tableName) => null;

        /// <summary>
        /// Does not compose the statement that checks whether the column exists, so the existence of the column is not known.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="columnName">The name of the column.</param>
        /// <returns><c>null</c>.</returns>
        public string ComposeColumnExists(string tableName, string columnName) => null;

        /// <summary>
        /// Does not compose the statement that checks whether the index exists, so the existence of the index is not known.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="indexName">The name of the index.</param>
        /// <returns><c>null</c>.</returns>
        public string ComposeIndexExists(string tableName, string indexName) => null;

        /// <summary>
        /// Does not compose the statement that checks whether the name of the index is used, so the name of the index is not checked.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="indexName">The name of the index.</param>
        /// <returns><c>null</c>.</returns>
        public string ComposeIndexNameExists(string tableName, string indexName) => null;

        /// <summary>
        /// Composes the data type of a column (including its size, precision and scale) in the dialect of the destination database.
        /// </summary>
        /// <param name="column">The column (as read from the source database) to be mapped.</param>
        /// <returns>The destination data type.</returns>
        public string ComposeTypeName(ColumnInfo column) => _composer.ComposeTypeName(column);

        #endregion
    }
}
