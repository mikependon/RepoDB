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
    /// An <see cref="ISchemaComposer"/> that composes the SQL statements, in the dialect of SQLite, that create the objects of a table.
    /// The column types are mapped on a best-effort basis, so the schema can come from a SQLite database or from another database engine.
    /// SQLite can not add a foreign key to an existing table, so the foreign keys are part of the statement that creates the table (see <see cref="ComposeAddForeignKey"/>).
    /// </summary>
    public class SqliteSchemaComposer : ISchemaComposer
    {
        #region Private Variables

        private readonly DbTypeNameToSqliteTypeNameResolver _typeNameResolver = new DbTypeNameToSqliteTypeNameResolver();
        private readonly ClientTypeToSqliteTypeNameResolver _clientTypeResolver = new ClientTypeToSqliteTypeNameResolver();

        #endregion

        #region Public Methods

        /// <summary>
        /// Composes the whole script that creates the table, its indexes and its foreign keys, as an ordered list of statements.
        /// The foreign keys are created by the statement that creates the table.
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
            return statements;
        }

        /// <summary>
        /// Composes the whole script that creates the tables, their indexes and their foreign keys, as an ordered list of statements.
        /// SQLite does not check the tables that a foreign key references when the table is created, so the tables can be given in any order
        /// and can reference each other (even in a circular way).
        /// The statements are ordered like this: one statement for each table (in the given order), then one statement for each index
        /// (the tables in the given order), then one statement for each foreign key (the tables in the given order). A foreign key is already
        /// created by the statement of its table, so the statement of a foreign key is empty (an empty statement is not executed).
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
                statements.AddRange(schema.ForeignKeys.Select(_ => string.Empty));
            }
            return statements;
        }

        /// <summary>
        /// Composes the statement that creates the table, including its columns, primary key, unique constraints, check constraints and foreign keys.
        /// </summary>
        /// <param name="schema">The schema of the table.</param>
        /// <returns>The SQL statement.</returns>
        public string ComposeCreateTable(TableSchema schema)
        {
            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            var inlineKey = GetInlineKey(schema);
            var definitions = new List<string>();
            definitions.AddRange(schema.Columns.OrderBy(c => c.Ordinal).Select(c =>
                Column(c, inlineKey != null && string.Equals(c.Field.Name, inlineKey, StringComparison.Ordinal) ? schema.PrimaryKey : null)));
            if (inlineKey == null && schema.PrimaryKey != null && schema.PrimaryKey.Columns.Count > 0)
            {
                definitions.Add($"{ConstraintName(schema.PrimaryKey.Name)}PRIMARY KEY ({Columns(schema.PrimaryKey.Columns)})");
            }
            definitions.AddRange(schema.UniqueConstraints.Select(u =>
                $"{ConstraintName(IsGeneratedName(u.Name) ? null : u.Name)}UNIQUE ({Columns(u.Columns)})"));
            definitions.AddRange(schema.CheckConstraints.Select(c =>
                $"{ConstraintName(c.Name)}CHECK ({c.Expression})"));
            definitions.AddRange(schema.ForeignKeys.Select(ForeignKey));

            return $"CREATE TABLE {Name(TableName(schema))} ({Environment.NewLine}    {string.Join("," + Environment.NewLine + "    ", definitions)}{Environment.NewLine});";
        }

        /// <summary>
        /// Composes the statement that creates an index.
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

            var (schema, table) = SqliteSchemaHelper.ParseSchemaAndTable(tableName);
            var statement = new StringBuilder("CREATE ");
            if (index.IsUnique)
            {
                statement.Append("UNIQUE ");
            }
            var keys = string.Join(", ", index.Columns.Select(column =>
                index.DescendingColumns != null && index.DescendingColumns.Contains(column, StringComparer.Ordinal)
                    ? $"{Quote(column)} DESC"
                    : Quote(column)));
            statement.Append($"INDEX {(schema == null ? string.Empty : Quote(schema) + ".")}{Quote(index.Name)} ON {Quote(table)} ({keys})");
            if (!string.IsNullOrWhiteSpace(index.Filter))
            {
                statement.Append($" WHERE {index.Filter}");
            }
            return statement.Append(';').ToString();
        }

        /// <summary>
        /// Composes the statement that adds a foreign key. SQLite can not add a foreign key to an existing table (the foreign keys are created together with the table),
        /// so the statement is empty.
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
            return $"ALTER TABLE {Name(tableName)} ADD COLUMN {Column(column, null)};";
        }

        /// <summary>
        /// Composes the statement that drops the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The SQL statement.</returns>
        public string ComposeDropTable(string tableName) =>
            $"DROP TABLE IF EXISTS {Name(tableName)};";

        /// <summary>
        /// Composes the name of the table, in the dialect of the destination database, so it can be given to the other methods that take the name of a table.
        /// </summary>
        /// <param name="table">The identity (name and schema) of the table.</param>
        /// <returns>The name of the table.</returns>
        public string ComposeName(TableInfo table) =>
            SqliteSchemaHelper.FormatTableName((table ?? throw new ArgumentNullException(nameof(table))).Schema, table.Name);

        /// <summary>
        /// Composes the statement that checks whether the table exists in the destination database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The SQL statement, that returns <c>1</c> if the table exists, and <c>0</c> if not.</returns>
        public string ComposeTableExists(string tableName)
        {
            var (schema, table) = SqliteSchemaHelper.ParseSchemaAndTable(tableName);
            return $"SELECT CASE WHEN EXISTS (SELECT 1 FROM {Quote(schema ?? SqliteSchemaHelper.MainSchema)}.sqlite_master WHERE type = 'table' AND name = '{Literal(table)}' COLLATE NOCASE) THEN 1 ELSE 0 END;";
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
            var (schema, table) = SqliteSchemaHelper.ParseSchemaAndTable(tableName);
            return $"SELECT CASE WHEN EXISTS (SELECT 1 FROM pragma_table_xinfo('{Literal(table)}', '{Literal(schema ?? SqliteSchemaHelper.MainSchema)}') WHERE name = '{Literal(columnName)}' COLLATE NOCASE) THEN 1 ELSE 0 END;";
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
            var (schema, table) = SqliteSchemaHelper.ParseSchemaAndTable(tableName);
            return $"SELECT CASE WHEN EXISTS (SELECT 1 FROM {Quote(schema ?? SqliteSchemaHelper.MainSchema)}.sqlite_master WHERE type = 'index' AND tbl_name = '{Literal(table)}' COLLATE NOCASE AND name = '{Literal(indexName)}' COLLATE NOCASE) THEN 1 ELSE 0 END;";
        }

        /// <summary>
        /// Composes the data type of a column (including its size, precision and scale) in the dialect of the destination database.
        /// A column that comes from SQLite keeps the type that it declares.
        /// </summary>
        /// <param name="column">The column (as read from the source database) to be mapped.</param>
        /// <returns>The destination data type, i.e.: <c>varchar(128)</c>. It is empty if the column declares no type.</returns>
        public string ComposeTypeName(ColumnInfo column)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }

            var field = column.Field ?? throw new ArgumentException("The column has no field.", nameof(column));
            var isSqlite = string.Equals(field.Provider, "SQLite", StringComparison.Ordinal);
            var type = isSqlite ? field.DatabaseType : _typeNameResolver.Resolve(field.DatabaseType) ?? _clientTypeResolver.Resolve(field.Type);
            if (string.IsNullOrWhiteSpace(type))
            {
                return string.Empty;
            }

            if (!isSqlite && type == "uuid")
            {
                return "char(36)";
            }
            switch (type)
            {
                case "decimal":
                case "numeric":
                case "dec":
                case "float":
                case "double":
                case "real":
                    return field.Precision > 0
                        ? field.Scale > 0 ? $"{type}({field.Precision},{field.Scale})" : $"{type}({field.Precision})"
                        : type;
                default:
                    return field.Size > 0 && (isSqlite || IsSizedType(type)) ? $"{type}({field.Size})" : type;
            }
        }

        #endregion

        #region Helpers

        // Names

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string Quote(string name) =>
            SqliteSchemaHelper.Quote(name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string Name(string name) =>
            SqliteSchemaHelper.QuoteName(name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string Literal(string value) =>
            value.Replace("'", "''");

        /// <summary>
        ///
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        private static string TableName(TableSchema schema) =>
            SqliteSchemaHelper.FormatTableName(schema.Table.Schema, schema.Table.Name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        private static string Columns(IEnumerable<string> columns) =>
            string.Join(", ", columns.Select(Quote));

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string ConstraintName(string name) =>
            string.IsNullOrWhiteSpace(name) ? string.Empty : $"CONSTRAINT {Quote(name)} ";

        /// <summary>
        /// Checks whether the name is the name that SQLite gives to the index of an unnamed constraint.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static bool IsGeneratedName(string name) =>
            name != null && name.StartsWith("sqlite_autoindex", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Checks whether the type takes a size, when it comes from another database engine.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsSizedType(string type) =>
            type == "varchar" || type == "char" || type == "binary" || type == "varbinary";

        /// <summary>
        ///
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        private static string Rule(CopySchemaForeignKeyRule rule)
        {
            switch (rule)
            {
                case CopySchemaForeignKeyRule.Cascade: return "CASCADE";
                case CopySchemaForeignKeyRule.SetNull: return "SET NULL";
                case CopySchemaForeignKeyRule.SetDefault: return "SET DEFAULT";
                case CopySchemaForeignKeyRule.Restrict: return "RESTRICT";
                default: return null;
            }
        }

        /// <summary>
        /// Gets the name of the column that is the primary key and is auto incremented. SQLite defines such a key together with the column.
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        private static string GetInlineKey(TableSchema schema)
        {
            if (schema.PrimaryKey == null || schema.PrimaryKey.Columns.Count != 1)
            {
                return null;
            }
            var name = schema.PrimaryKey.Columns[0];
            var column = schema.Columns.FirstOrDefault(c => string.Equals(c.Field.Name, name, StringComparison.Ordinal));
            return column != null && column.Field.IsIdentity ? name : null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="foreignKey"></param>
        /// <returns></returns>
        private static string ForeignKey(ForeignKeyInfo foreignKey)
        {
            var definition = new StringBuilder(
                $"{ConstraintName(foreignKey.Name)}FOREIGN KEY ({Columns(foreignKey.Columns)}) " +
                $"REFERENCES {Quote(foreignKey.ReferencedTable.Name)} ({Columns(foreignKey.ReferencedColumns)})");
            var onDelete = Rule(foreignKey.DeleteRule);
            if (onDelete != null)
            {
                definition.Append($" ON DELETE {onDelete}");
            }
            var onUpdate = Rule(foreignKey.UpdateRule);
            if (onUpdate != null)
            {
                definition.Append($" ON UPDATE {onUpdate}");
            }
            return definition.ToString();
        }

        // Columns

        /// <summary>
        ///
        /// </summary>
        /// <param name="column"></param>
        /// <param name="inlineKey">The primary key to be defined together with the column (an auto incremented column).</param>
        /// <returns></returns>
        private string Column(ColumnInfo column,
            PrimaryKeyInfo inlineKey)
        {
            var field = column.Field;
            var definition = new StringBuilder(Quote(field.Name));

            var type = inlineKey != null ? "integer" : ComposeTypeName(column);
            if (type.Length > 0)
            {
                definition.Append(' ').Append(type);
            }
            if (!string.IsNullOrWhiteSpace(column.ComputedExpression))
            {
                definition.Append(" GENERATED ALWAYS AS (").Append(column.ComputedExpression).Append(')');
            }
            if (!field.IsNullable || inlineKey != null)
            {
                definition.Append(" NOT NULL");
            }
            if (inlineKey != null)
            {
                definition.Append(' ').Append(ConstraintName(inlineKey.Name)).Append("PRIMARY KEY AUTOINCREMENT");
            }
            else if (string.IsNullOrWhiteSpace(column.ComputedExpression) && !string.IsNullOrWhiteSpace(column.DefaultExpression))
            {
                definition.Append(" DEFAULT ").Append(column.DefaultExpression);
            }
            if (!string.IsNullOrWhiteSpace(column.Collation))
            {
                definition.Append(" COLLATE ").Append(column.Collation);
            }
            return definition.ToString();
        }

        #endregion
    }
}
