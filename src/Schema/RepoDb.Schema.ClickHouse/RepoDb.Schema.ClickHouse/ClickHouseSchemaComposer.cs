#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RepoDb.Resolvers;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// An <see cref="ISchemaComposer"/> that composes the SQL statements, in the dialect of ClickHouse, that create the objects of a table.
    /// The column types are mapped on a best-effort basis, so the schema can come from a ClickHouse database or from another database engine.
    /// A statement has no terminator. ClickHouse has no transactions, no foreign keys, no unique constraints and no identity column, so those are not composed
    /// (the statement of a foreign key is empty). The table is a <c>MergeTree</c> that is ordered by its primary key, and an index is a data skipping index.
    /// </summary>
    public class ClickHouseSchemaComposer : ISchemaComposer
    {
        #region Private Variables

        private readonly DbTypeNameToClickHouseTypeNameResolver _typeNameResolver = new DbTypeNameToClickHouseTypeNameResolver();
        private readonly ClientTypeToClickHouseTypeNameResolver _clientTypeResolver = new ClientTypeToClickHouseTypeNameResolver();

        #endregion

        #region Public Methods

        /// <summary>
        /// Composes the whole script that creates the table, its indexes and its foreign keys, as an ordered list of statements.
        /// </summary>
        /// <param name="schema">The schema of the table.</param>
        /// <returns>The ordered SQL statements. Execute them in order.</returns>
        public IEnumerable<string> ComposeSchema(TableSchema schema)
        {
            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            var tableName = TableName(schema);
            var statements = new List<string> { ComposeCreateTable(schema) };
            statements.AddRange(schema.Indexes.Select(index => ComposeCreateIndex(tableName, index)));
            statements.AddRange(schema.ForeignKeys.Select(foreignKey => ComposeAddForeignKey(tableName, foreignKey)));
            return statements;
        }

        /// <summary>
        /// Composes the whole script that creates the tables, their indexes and their foreign keys, as an ordered list of statements.
        /// The statements are ordered like this: one statement for each table (in the given order), then one statement for each index
        /// (the tables in the given order), then one statement for each foreign key (the tables in the given order), which is empty as ClickHouse has no foreign key.
        /// </summary>
        /// <param name="schemas">The schemas of the tables.</param>
        /// <returns>The ordered SQL statements. Execute them in order.</returns>
        public IEnumerable<string> ComposeSchemas(IEnumerable<TableSchema> schemas)
        {
            if (schemas == null)
            {
                throw new ArgumentNullException(nameof(schemas));
            }

            var list = schemas.ToList();
            var statements = new List<string>();
            statements.AddRange(list.Select(ComposeCreateTable));
            foreach (var schema in list)
            {
                var tableName = TableName(schema);
                statements.AddRange(schema.Indexes.Select(index => ComposeCreateIndex(tableName, index)));
            }
            foreach (var schema in list)
            {
                var tableName = TableName(schema);
                statements.AddRange(schema.ForeignKeys.Select(foreignKey => ComposeAddForeignKey(tableName, foreignKey)));
            }
            return statements;
        }

        /// <summary>
        /// Composes the statement that creates the table, including its columns and its check constraints. The table is a <c>MergeTree</c> that is ordered by the primary key
        /// (the primary key of ClickHouse is not unique).
        /// </summary>
        /// <param name="schema">The schema of the table.</param>
        /// <returns>The SQL statement.</returns>
        public string ComposeCreateTable(TableSchema schema)
        {
            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            var definitions = new List<string>();
            definitions.AddRange(schema.Columns.OrderBy(c => c.Ordinal).Select(Column));
            definitions.AddRange(schema.CheckConstraints.Select(c => $"CONSTRAINT {Quote(c.Name)} CHECK {c.Expression}"));

            var key = schema.PrimaryKey == null || schema.PrimaryKey.Columns.Count == 0
                ? "tuple()"
                : schema.PrimaryKey.Columns.Count == 1 ? Quote(schema.PrimaryKey.Columns[0]) : $"({Columns(schema.PrimaryKey.Columns)})";
            return $"CREATE TABLE {Name(TableName(schema))} ({Environment.NewLine}    {string.Join("," + Environment.NewLine + "    ", definitions)}{Environment.NewLine}) ENGINE = MergeTree ORDER BY {key}";
        }

        /// <summary>
        /// Composes the statement that creates an index (a data skipping index of the type <c>minmax</c>). ClickHouse has no unique index,
        /// no direction of the keys, no included columns and no filter.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="index">The index to be created.</param>
        /// <returns>The SQL statement.</returns>
        public string ComposeCreateIndex(string tableName,
            IndexInfo index)
        {
            if (index == null)
            {
                throw new ArgumentNullException(nameof(index));
            }
            return $"ALTER TABLE {Name(tableName)} ADD INDEX {Quote(index.Name)} ({Columns(index.Columns)}) TYPE minmax GRANULARITY 1";
        }

        /// <summary>
        /// Composes the statement that adds a foreign key. ClickHouse has no foreign key, so the statement is empty (an empty statement is not executed).
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="foreignKey">The foreign key to be added.</param>
        /// <returns>An empty statement.</returns>
        public string ComposeAddForeignKey(string tableName,
            ForeignKeyInfo foreignKey)
        {
            if (foreignKey == null)
            {
                throw new ArgumentNullException(nameof(foreignKey));
            }
            return string.Empty;
        }

        /// <summary>
        /// Composes the statement that adds a column to an existing table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="column">The column to be added.</param>
        /// <returns>The SQL statement.</returns>
        public string ComposeAddColumn(string tableName,
            ColumnInfo column)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }
            return $"ALTER TABLE {Name(tableName)} ADD COLUMN {Column(column)}";
        }

        /// <summary>
        /// Composes the statement that drops the table (and waits for it), if it exists.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The SQL statement.</returns>
        public string ComposeDropTable(string tableName) =>
            $"DROP TABLE IF EXISTS {Name(tableName)} SYNC";

        /// <summary>
        /// Composes the name of the table, in the dialect of the destination database, so it can be given to the other methods that take the name of a table.
        /// </summary>
        /// <param name="table">The identity (name and schema) of the table.</param>
        /// <returns>The name of the table.</returns>
        public string ComposeName(TableInfo table) =>
            ClickHouseSchemaHelper.FormatTableName((table ?? throw new ArgumentNullException(nameof(table))).Schema, table.Name);

        /// <summary>
        /// Composes the statement that checks whether the table exists in the destination database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The SQL statement, that returns <c>1</c> if the table exists, and <c>0</c> if not.</returns>
        public string ComposeTableExists(string tableName) =>
            $"SELECT toUInt8(count() > 0) FROM system.tables WHERE {TableFilter(tableName, "database", "name")}";

        /// <summary>
        /// Composes the statement that checks whether a column exists in a table of the destination database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="columnName">The name of the column.</param>
        /// <returns>The SQL statement, that returns <c>1</c> if the column exists, and <c>0</c> if not.</returns>
        public string ComposeColumnExists(string tableName,
            string columnName) =>
            $"SELECT toUInt8(count() > 0) FROM system.columns WHERE {TableFilter(tableName, "database", "table")} AND name = '{Literal(columnName)}'";

        /// <summary>
        /// Composes the statement that checks whether an index exists in a table of the destination database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="indexName">The name of the index.</param>
        /// <returns>The SQL statement, that returns <c>1</c> if the index exists, and <c>0</c> if not.</returns>
        public string ComposeIndexExists(string tableName,
            string indexName) =>
            $"SELECT toUInt8(count() > 0) FROM system.data_skipping_indices WHERE {TableFilter(tableName, "database", "table")} AND name = '{Literal(indexName)}'";

        /// <summary>
        /// Composes the data type of a column (including its size, precision and scale) in the dialect of the destination database.
        /// A column that comes from ClickHouse keeps the type that it declares. The column is wrapped in <c>Nullable</c> if it is nullable.
        /// </summary>
        /// <param name="column">The column (as read from the source database) to be mapped.</param>
        /// <returns>The destination data type, i.e.: <c>Nullable(String)</c>.</returns>
        public string ComposeTypeName(ColumnInfo column)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }

            var field = column.Field ?? throw new ArgumentException("The column has no field.", nameof(column));
            var isClickHouse = string.Equals(field.Provider, "ClickHouse", StringComparison.Ordinal);
            var type = isClickHouse ? field.DatabaseType : _typeNameResolver.Resolve(field.DatabaseType) ?? _clientTypeResolver.Resolve(field.Type);
            return Nullable(ComposeBaseTypeName(type, field), field.IsNullable);
        }

        #endregion

        #region Helpers

        // Types

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        private static string ComposeBaseTypeName(string type,
            DbField field)
        {
            switch (type.ToLowerInvariant())
            {
                case "fixedstring":
                    return field.Size > 0 ? $"FixedString({field.Size})" : "String";
                case "decimal":
                    return field.Precision > 0
                        ? $"Decimal({Math.Min((int)field.Precision, 76)}, {Math.Min((int)(field.Scale ?? 0), Math.Min((int)field.Precision, 76))})"
                        : "Decimal(38, 9)";
                case "datetime64":
                    return $"DateTime64({Math.Min((int)(field.Scale ?? 3), 9)})";
                default:
                    return type;
            }
        }

        /// <summary>
        /// Wraps the type in <c>Nullable</c> if the column is nullable. The types that can not be nullable (the collections) are not wrapped.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isNullable"></param>
        /// <returns></returns>
        private static string Nullable(string type,
            bool isNullable)
        {
            if (!isNullable ||
                type.StartsWith("Nullable(", StringComparison.Ordinal) ||
                type.StartsWith("Array(", StringComparison.Ordinal) ||
                type.StartsWith("Map(", StringComparison.Ordinal) ||
                type.StartsWith("Tuple(", StringComparison.Ordinal) ||
                type.StartsWith("Nested(", StringComparison.Ordinal))
            {
                return type;
            }
            return type.StartsWith("LowCardinality(", StringComparison.Ordinal)
                ? $"LowCardinality(Nullable({type.Substring("LowCardinality(".Length, type.Length - "LowCardinality(".Length - 1)}))"
                : $"Nullable({type})";
        }

        // Names

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string Quote(string name) =>
            ClickHouseSchemaHelper.Quote(name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string Name(string name) =>
            ClickHouseSchemaHelper.QuoteName(name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string Literal(string value) =>
            value.Replace("\\", "\\\\").Replace("'", "''");

        /// <summary>
        /// Gets the filter of the table in the <c>system</c> tables: the schema (database) is the one of the name, or the current database.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="schemaColumn"></param>
        /// <param name="tableColumn"></param>
        /// <returns></returns>
        private static string TableFilter(string tableName,
            string schemaColumn,
            string tableColumn)
        {
            var (schema, table) = ClickHouseSchemaHelper.ParseSchemaAndTable(tableName);
            var schemaValue = schema == null ? "currentDatabase()" : $"'{Literal(schema)}'";
            return $"{schemaColumn} = {schemaValue} AND {tableColumn} = '{Literal(table)}'";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        private static string TableName(TableSchema schema) =>
            ClickHouseSchemaHelper.FormatTableName(schema.Table.Schema, schema.Table.Name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        private static string Columns(IEnumerable<string> columns) =>
            string.Join(", ", columns.Select(Quote));

        // Columns

        /// <summary>
        ///
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private string Column(ColumnInfo column)
        {
            var field = column.Field;
            var definition = new StringBuilder(Quote(field.Name)).Append(' ').Append(ComposeTypeName(column));
            if (!string.IsNullOrWhiteSpace(column.ComputedExpression))
            {
                definition.Append(" MATERIALIZED ").Append(column.ComputedExpression);
            }
            else if (!string.IsNullOrWhiteSpace(column.DefaultExpression) && !field.IsIdentity)
            {
                definition.Append(" DEFAULT ").Append(column.DefaultExpression);
            }
            return definition.ToString();
        }

        #endregion
    }
}
