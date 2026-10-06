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
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// An <see cref="ISchemaComposer"/> that composes the SQL statements, in the dialect of DuckDb, that create the objects of a table.
    /// The column types are mapped on a best-effort basis, so the schema can come from a DuckDb database or from another database engine.
    /// A statement has no terminator, and a DDL statement is part of the transaction (it can be rolled back). DuckDB can not add a foreign key to an existing table, so the foreign keys are part of the statement that creates the table (see <see cref="ComposeAddForeignKey"/>), and the tables must be created after the tables that they reference. DuckDB has no identity column, so an identity column is a column with a sequence as its default. DuckDB does not keep the names of the constraints, so the constraints are not named.
    /// </summary>
    public class DuckDbSchemaComposer : ISchemaComposer
    {
        #region Private Variables

        private const string CurrentSchema = "current_schema()";
        private readonly DbTypeNameToDuckDbTypeNameResolver _typeNameResolver = new DbTypeNameToDuckDbTypeNameResolver();
        private readonly ClientTypeToDuckDbTypeNameResolver _clientTypeResolver = new ClientTypeToDuckDbTypeNameResolver();

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
        /// All the tables are created first, then all the indexes and then all the foreign keys, so the tables can be given in any order
        /// and can reference each other (even in a circular way) as long as the referenced tables are part of the given tables
        /// or already exist in the destination database.
        /// The statements are ordered like this: one statement for each table (in the given order), then one statement for each index
        /// (the tables in the given order), then one statement for each foreign key (the tables in the given order).
        /// </summary>
        /// <param name="schemas">The schemas of the tables.</param>
        /// <returns>The ordered SQL statements. Execute them in order.</returns>
        public IEnumerable<string> ComposeSchemas(IEnumerable<TableSchema> schemas)
        {
            if (schemas == null)
            {
                throw new ArgumentNullException(nameof(schemas));
            }

            var list = Sort(schemas.ToList());
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
        /// Composes the statement that creates the table, including its columns, primary key, unique constraints, check constraints and foreign keys.
        /// The statement of a table with identity columns starts with the statements that create their sequences (separated by a semicolon).
        /// </summary>
        /// <param name="schema">The schema of the table.</param>
        /// <returns>The SQL statement.</returns>
        public string ComposeCreateTable(TableSchema schema)
        {
            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            var tableName = TableName(schema);
            var definitions = new List<string>();
            definitions.AddRange(schema.Columns.OrderBy(c => c.Ordinal).Select(c => Column(tableName, c)));
            if (schema.PrimaryKey != null && schema.PrimaryKey.Columns.Count > 0)
            {
                definitions.Add($"PRIMARY KEY ({Columns(schema.PrimaryKey.Columns)})");
            }
            definitions.AddRange(schema.UniqueConstraints.Select(u => $"UNIQUE ({Columns(u.Columns)})"));
            definitions.AddRange(schema.CheckConstraints.Select(c => $"CHECK ({c.Expression})"));
            definitions.AddRange(schema.ForeignKeys.Select(f =>
                $"FOREIGN KEY ({Columns(f.Columns)}) REFERENCES {Name(TableName(f.ReferencedTable))} ({Columns(f.ReferencedColumns)})"));

            var statements = schema.Columns.Where(c => c.Field.IsIdentity).OrderBy(c => c.Ordinal).Select(c => CreateSequence(tableName, c)).ToList();
            statements.Add($"CREATE TABLE {Name(tableName)} ({Environment.NewLine}    {string.Join("," + Environment.NewLine + "    ", definitions)}{Environment.NewLine})");
            return string.Join("; ", statements);
        }

        /// <summary>
        /// Composes the statement that creates an index. The index is created in the schema of the table. DuckDB does not support the direction of the keys, the included columns and the filter.
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

            var statement = new StringBuilder("CREATE ");
            if (index.IsUnique)
            {
                statement.Append("UNIQUE ");
            }
            statement.Append($"INDEX {Quote(index.Name)} ON {Name(tableName)} ({Columns(index.Columns)})");
            return statement.ToString();
        }

        /// <summary>
        /// Composes the statement that adds a foreign key. DuckDB can not add a foreign key to an existing table (the foreign keys are created together with the table),
        /// so the statement is empty (an empty statement is not executed).
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
        /// Composes the statement that adds a column to an existing table. DuckDB can not add a column that has a constraint, so the statement of a column that is not nullable
        /// is a script of 2 statements that are separated by a semicolon (the column is set as not null after it is added).
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="column">The column to be added.</param>
        /// <returns>The SQL statement.</returns>
        /// <exception cref="NotSupportedException">The column is a generated column, as DuckDB can not add it to an existing table.</exception>
        public string ComposeAddColumn(string tableName,
            ColumnInfo column)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }
            if (!string.IsNullOrWhiteSpace(column.ComputedExpression))
            {
                throw new NotSupportedException("DuckDB can not add a generated column to an existing table.");
            }

            // DuckDB can not add a column with a constraint, so the column is added first and it is set as not null after
            var field = column.Field;
            var statements = new List<string>();
            if (field.IsIdentity)
            {
                statements.Add(CreateSequence(tableName, column));
            }
            var definition = new StringBuilder($"ALTER TABLE {Name(tableName)} ADD COLUMN {Quote(field.Name)} {ComposeTypeName(column)}");
            var defaultExpression = field.IsIdentity ? $"nextval('{SequenceName(tableName, column).Replace("'", "''")}')" : column.DefaultExpression;
            if (!string.IsNullOrWhiteSpace(defaultExpression))
            {
                definition.Append(" DEFAULT ").Append(defaultExpression);
            }
            statements.Add(definition.ToString());
            if (!field.IsNullable)
            {
                statements.Add($"ALTER TABLE {Name(tableName)} ALTER COLUMN {Quote(field.Name)} SET NOT NULL");
            }
            return string.Join("; ", statements);
        }

        /// <summary>
        /// Composes the statement that drops the table (with its indexes, and the tables that reference it), if it exists.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The SQL statement.</returns>
        public string ComposeDropTable(string tableName) =>
            $"DROP TABLE IF EXISTS {Name(tableName)} CASCADE";

        /// <summary>
        /// Composes the name of the table, in the dialect of the destination database, so it can be given to the other methods that take the name of a table.
        /// </summary>
        /// <param name="table">The identity (name and schema) of the table.</param>
        /// <returns>The name of the table.</returns>
        public string ComposeName(TableInfo table) =>
            DuckDbSchemaHelper.FormatTableName((table ?? throw new ArgumentNullException(nameof(table))).Schema, table.Name);

        /// <summary>
        /// Composes the statement that checks whether the table exists in the destination database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The SQL statement, that returns <c>1</c> if the table exists, and <c>0</c> if not.</returns>
        public string ComposeTableExists(string tableName)
        {
            var (schema, table) = DuckDbSchemaHelper.ParseSchemaAndTable(tableName);
            return $"SELECT CASE WHEN EXISTS (SELECT 1 FROM duckdb_tables() WHERE database_name = current_database() AND schema_name = {Owner(schema)} AND table_name = '{Literal(table)}') THEN 1 ELSE 0 END";
        }

        /// <summary>
        /// Composes the statement that checks whether a column exists in a table of the destination database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="columnName">The name of the column.</param>
        /// <returns>The SQL statement, that returns <c>1</c> if the column exists, and <c>0</c> if not.</returns>
        public string ComposeColumnExists(string tableName,
            string columnName)
        {
            var (schema, table) = DuckDbSchemaHelper.ParseSchemaAndTable(tableName);
            return $"SELECT CASE WHEN EXISTS (SELECT 1 FROM duckdb_columns() WHERE database_name = current_database() AND schema_name = {Owner(schema)} AND table_name = '{Literal(table)}' AND column_name = '{Literal(columnName)}') THEN 1 ELSE 0 END";
        }

        /// <summary>
        /// Composes the statement that checks whether an index exists in a table of the destination database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="indexName">The name of the index.</param>
        /// <returns>The SQL statement, that returns <c>1</c> if the index exists, and <c>0</c> if not.</returns>
        public string ComposeIndexExists(string tableName,
            string indexName)
        {
            var (schema, table) = DuckDbSchemaHelper.ParseSchemaAndTable(tableName);
            return $"SELECT CASE WHEN EXISTS (SELECT 1 FROM duckdb_indexes() WHERE database_name = current_database() AND schema_name = {Owner(schema)} AND table_name = '{Literal(table)}' AND index_name = '{Literal(indexName)}') THEN 1 ELSE 0 END";
        }

        /// <summary>
        /// Composes the data type of a column (including its size, precision and scale) in the dialect of the destination database.
        /// A column that comes from DuckDb keeps the type that it declares.
        /// </summary>
        /// <param name="column">The column (as read from the source database) to be mapped.</param>
        /// <returns>The destination data type, i.e.: <c>VARCHAR(128)</c>.</returns>
        public string ComposeTypeName(ColumnInfo column)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }

            var field = column.Field ?? throw new ArgumentException("The column has no field.", nameof(column));
            var isDuckDb = string.Equals(field.Provider, "DuckDb", StringComparison.Ordinal);
            var type = isDuckDb ? (IsSimple(field.DatabaseType) ? field.DatabaseType.ToLowerInvariant() : field.DatabaseType) : _typeNameResolver.Resolve(field.DatabaseType) ?? _clientTypeResolver.Resolve(field.Type);

            switch (type)
            {
                case "decimal":
                case "numeric":
                    return field.Precision > 0 ? $"DECIMAL({Math.Min((int)field.Precision, 38)},{Math.Min((int)(field.Scale ?? 0), Math.Min((int)field.Precision, 38))})" : "DECIMAL(18,3)";
                default:
                    return isDuckDb && !IsSimple(type) ? field.DatabaseType : type.ToUpperInvariant();
            }
        }

        #endregion

        #region Helpers

        // Ordering

        /// <summary>
        /// Sorts the tables so that a table comes after the tables that it references, as the foreign keys are created with the table (the given order is kept when it is already valid).
        /// The tables that reference each other (in a circular way) are kept in the given order, as DuckDB can not create them.
        /// </summary>
        /// <param name="schemas"></param>
        /// <returns></returns>
        private static List<TableSchema> Sort(List<TableSchema> schemas)
        {
            var remaining = new List<TableSchema>(schemas);
            var sorted = new List<TableSchema>();
            while (remaining.Count > 0)
            {
                var next = remaining.FirstOrDefault(schema => schema.ForeignKeys.All(foreignKey =>
                    IsSameTable(foreignKey.ReferencedTable, schema.Table) ||
                    sorted.Any(s => IsSameTable(foreignKey.ReferencedTable, s.Table)) ||
                    !remaining.Any(r => IsSameTable(foreignKey.ReferencedTable, r.Table)))) ?? remaining[0];
                sorted.Add(next);
                remaining.Remove(next);
            }
            return sorted;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private static bool IsSameTable(TableInfo first,
            TableInfo second) =>
            string.Equals(first.Name, second.Name, StringComparison.Ordinal) && string.Equals(first.Schema, second.Schema, StringComparison.Ordinal);

        // Types

        /// <summary>
        /// Checks whether the type is a simple one (its name has no other type or value in it).
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsSimple(string type) =>
            type.IndexOf('(') < 0 && type.IndexOf('[') < 0;

        // Names

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string Quote(string name) =>
            DuckDbSchemaHelper.Quote(name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string Name(string name) =>
            DuckDbSchemaHelper.QuoteName(name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string Literal(string value) =>
            value.Replace("'", "''");

        /// <summary>
        /// Gets the expression of the schema in the catalog: the schema, or the current schema of the connection.
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        private static string Owner(string schema) =>
            schema == null ? CurrentSchema : $"'{Literal(schema)}'";

        /// <summary>
        ///
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        private static string TableName(TableSchema schema) =>
            DuckDbSchemaHelper.FormatTableName(schema.Table.Schema, schema.Table.Name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static string TableName(TableInfo table) =>
            DuckDbSchemaHelper.FormatTableName(table.Schema, table.Name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        private static string Columns(IEnumerable<string> columns) =>
            string.Join(", ", columns.Select(Quote));

        // Sequences

        /// <summary>
        /// Gets the name of the sequence of an identity column, in the schema of the table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private static string SequenceName(string tableName,
            ColumnInfo column)
        {
            var (schema, table) = DuckDbSchemaHelper.ParseSchemaAndTable(tableName);
            var name = $"{table}_{column.Field.Name}_seq";
            return schema == null ? Quote(name) : DuckDbSchemaHelper.QuoteSchemaAndTable(schema, name);
        }

        /// <summary>
        /// Composes the statement that creates the sequence of an identity column.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private static string CreateSequence(string tableName,
            ColumnInfo column) =>
            $"CREATE OR REPLACE SEQUENCE {SequenceName(tableName, column)} START {column.IdentitySeed ?? 1} INCREMENT {column.IdentityIncrement ?? 1}";

        // Columns

        /// <summary>
        ///
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private string Column(string tableName,
            ColumnInfo column)
        {
            var field = column.Field;
            var definition = new StringBuilder(Quote(field.Name)).Append(' ').Append(ComposeTypeName(column));
            if (!string.IsNullOrWhiteSpace(column.ComputedExpression))
            {
                return definition.Append(" GENERATED ALWAYS AS (").Append(column.ComputedExpression).Append(')').ToString();
            }
            if (field.IsIdentity)
            {
                definition.Append(" DEFAULT nextval('").Append(SequenceName(tableName, column).Replace("'", "''")).Append("')");
            }
            else if (!string.IsNullOrWhiteSpace(column.DefaultExpression))
            {
                definition.Append(" DEFAULT ").Append(column.DefaultExpression);
            }
            return definition.Append(field.IsNullable ? string.Empty : " NOT NULL").ToString();
        }

        #endregion
    }
}
