#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// An interface that is used to define the schema composer of the library. A schema composer turns the schema information
    /// read by an <see cref="ISchemaReader"/> into the SQL statements, in the dialect of the destination database, that create the equivalent objects.
    /// </summary>
    public interface ISchemaComposer
    {
        /// <summary>
        /// Composes the whole script that creates the table, its indexes and its foreign keys, as an ordered list of statements.
        /// </summary>
        /// <param name="schema">The schema of the table.</param>
        /// <returns>The ordered SQL statements. Execute them in order.</returns>
        IEnumerable<string> ComposeSchema(TableSchema schema);

        /// <summary>
        /// Composes the whole script that creates the tables, their indexes and their foreign keys, as an ordered list of statements.
        /// All the tables are created first, then all the indexes and then all the foreign keys, so the tables can be given in any order
        /// and can reference each other (even in a circular way) as long as the referenced tables are part of the given tables
        /// or already exist in the destination database.
        /// The statements are ordered like this: one statement for each table (in the given order), then one statement for each index
        /// (the tables in the given order), then one statement for each foreign key (the tables in the given order).
        /// </summary>
        /// <param name="schemas">The schemas of the tables.</param>
        /// <returns>The ordered SQL statements. Execute them in order.</returns>
        IEnumerable<string> ComposeSchemas(IEnumerable<TableSchema> schemas);

        /// <summary>
        /// Composes the statement that creates the table, including its columns, primary key, unique constraints and check constraints.
        /// </summary>
        /// <param name="schema">The schema of the table.</param>
        /// <returns>The SQL statement.</returns>
        string ComposeCreateTable(TableSchema schema);

        /// <summary>
        /// Composes the statement that creates an index.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="index">The index to be created.</param>
        /// <returns>The SQL statement.</returns>
        string ComposeCreateIndex(string tableName, IndexInfo index);

        /// <summary>
        /// Composes the statement that adds a foreign key.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="foreignKey">The foreign key to be added.</param>
        /// <returns>The SQL statement.</returns>
        string ComposeAddForeignKey(string tableName, ForeignKeyInfo foreignKey);

        /// <summary>
        /// Composes the statement that adds a column to an existing table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="column">The column to be added.</param>
        /// <returns>The SQL statement.</returns>
        string ComposeAddColumn(string tableName, ColumnInfo column);

        /// <summary>
        /// Composes the statement that drops the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The SQL statement.</returns>
        string ComposeDropTable(string tableName);

        /// <summary>
        /// Composes the name of the table, in the dialect of the destination database, so it can be given to the other methods that take the name of a table.
        /// </summary>
        /// <param name="table">The identity (name and schema) of the table.</param>
        /// <returns>The name of the table, i.e.: <c>dbo.Person</c> or <c>public."Order Details"</c>.</returns>
        string ComposeName(TableInfo table);

        /// <summary>
        /// Composes the statement that checks whether the table exists in the destination database.
        /// </summary>
        /// <param name="tableName">The name of the table (see <see cref="ComposeName"/>).</param>
        /// <returns>The SQL statement, that returns <c>1</c> if the table exists, and <c>0</c> if not.</returns>
        string ComposeTableExists(string tableName);

        /// <summary>
        /// Composes the statement that checks whether a column exists in a table of the destination database.
        /// </summary>
        /// <param name="tableName">The name of the table (see <see cref="ComposeName"/>).</param>
        /// <param name="columnName">The name of the column.</param>
        /// <returns>The SQL statement, that returns <c>1</c> if the column exists, and <c>0</c> if not.</returns>
        string ComposeColumnExists(string tableName, string columnName);

        /// <summary>
        /// Composes the statement that checks whether an index exists in a table of the destination database.
        /// </summary>
        /// <param name="tableName">The name of the table (see <see cref="ComposeName"/>).</param>
        /// <param name="indexName">The name of the index.</param>
        /// <returns>The SQL statement, that returns <c>1</c> if the index exists, and <c>0</c> if not.</returns>
        string ComposeIndexExists(string tableName, string indexName);

        /// <summary>
        /// Composes the data type of a column (including its size, precision and scale) in the dialect of the destination database.
        /// </summary>
        /// <param name="column">The column (as read from the source database) to be mapped.</param>
        /// <returns>The destination data type, i.e.: <c>VARCHAR(128)</c>.</returns>
        string ComposeTypeName(ColumnInfo column);
    }
}
