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
    /// An <see cref="ISchemaComposer"/> that composes the SQL statements, in the dialect of MariaDB, that create the objects of a table.
    /// The column types are mapped on a best-effort basis, so the schema can come from a MariaDB database or from another database engine.
    /// </summary>
    public class MariaDbSchemaComposer : ISchemaComposer
    {
        #region Private Variables

        private readonly DbTypeNameToMariaDbTypeNameResolver _typeNameResolver = new DbTypeNameToMariaDbTypeNameResolver();
        private readonly ClientTypeToMariaDbTypeNameResolver _clientTypeResolver = new ClientTypeToMariaDbTypeNameResolver();

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
        /// Composes the statement that creates the table, including its columns, primary key, unique constraints and check constraints.
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
            if (schema.PrimaryKey != null && schema.PrimaryKey.Columns.Count > 0)
            {
                definitions.Add($"PRIMARY KEY ({Columns(schema.PrimaryKey.Columns)})");
            }
            definitions.AddRange(schema.UniqueConstraints.Select(u =>
                $"UNIQUE KEY {(string.IsNullOrWhiteSpace(u.Name) ? string.Empty : Quote(u.Name) + " ")}({Columns(u.Columns)})"));
            definitions.AddRange(schema.CheckConstraints.Select(c =>
                $"{ConstraintName(c.Name)}CHECK ({c.Expression})"));

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
            var statement = new StringBuilder("CREATE ");
            if (index.IsUnique)
            {
                statement.Append("UNIQUE ");
            }
            var keys = string.Join(", ", index.Columns.Select(column =>
                index.DescendingColumns != null && index.DescendingColumns.Contains(column, StringComparer.Ordinal)
                    ? $"{Quote(column)} DESC"
                    : Quote(column)));
            statement.Append($"INDEX {Quote(index.Name)} ON {Name(tableName)} ({keys})");
            return statement.Append(';').ToString();
        }

        /// <summary>
        /// Composes the statement that adds a foreign key.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="foreignKey">The foreign key to be added.</param>
        /// <returns>The SQL statement.</returns>
        public string ComposeAddForeignKey(string tableName,
            ForeignKeyInfo foreignKey)
        {
            if (foreignKey == null)
            {
                throw new ArgumentNullException(nameof(foreignKey));
            }
            var statement = new StringBuilder(
                $"ALTER TABLE {Name(tableName)} ADD {ConstraintName(foreignKey.Name)}FOREIGN KEY ({Columns(foreignKey.Columns)}) " +
                $"REFERENCES {Name(TableName(foreignKey.ReferencedTable))} ({Columns(foreignKey.ReferencedColumns)})");
            var onDelete = Rule(foreignKey.DeleteRule);
            if (onDelete != null)
            {
                statement.Append($" ON DELETE {onDelete}");
            }
            var onUpdate = Rule(foreignKey.UpdateRule);
            if (onUpdate != null)
            {
                statement.Append($" ON UPDATE {onUpdate}");
            }
            return statement.Append(';').ToString();
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
            return $"ALTER TABLE {Name(tableName)} ADD COLUMN {Column(column)};";
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
            MariaDbSchemaHelper.Format((table ?? throw new ArgumentNullException(nameof(table))).Schema, table.Name);

        /// <summary>
        /// Composes the statement that checks whether the table exists in the destination database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The SQL statement, that returns <c>1</c> if the table exists, and <c>0</c> if not.</returns>
        public string ComposeTableExists(string tableName) =>
            $"SELECT CASE WHEN EXISTS (SELECT 1 FROM information_schema.TABLES WHERE {TableFilter(tableName, "TABLE_SCHEMA", "TABLE_NAME")} AND TABLE_TYPE = 'BASE TABLE') THEN 1 ELSE 0 END;";

        /// <summary>
        /// Composes the statement that checks whether a column exists in a table of the destination database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="columnName">The name of the column.</param>
        /// <returns>The SQL statement, that returns <c>1</c> if the column exists, and <c>0</c> if not.</returns>
        public string ComposeColumnExists(string tableName,
            string columnName) =>
            $"SELECT CASE WHEN EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE {TableFilter(tableName, "TABLE_SCHEMA", "TABLE_NAME")} AND COLUMN_NAME = '{Literal(columnName)}') THEN 1 ELSE 0 END;";

        /// <summary>
        /// Composes the statement that checks whether an index exists in a table of the destination database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="indexName">The name of the index.</param>
        /// <returns>The SQL statement, that returns <c>1</c> if the index exists, and <c>0</c> if not.</returns>
        public string ComposeIndexExists(string tableName,
            string indexName) =>
            $"SELECT CASE WHEN EXISTS (SELECT 1 FROM information_schema.STATISTICS WHERE {TableFilter(tableName, "TABLE_SCHEMA", "TABLE_NAME")} AND INDEX_NAME = '{Literal(indexName)}') THEN 1 ELSE 0 END;";

        /// <summary>
        /// Composes the data type of a column (including its size, precision and scale) in the dialect of the destination database.
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
            var type = _typeNameResolver.Resolve(field.DatabaseType) ?? _clientTypeResolver.Resolve(field.Type);

            var unsigned = type != null && type.EndsWith(" unsigned", StringComparison.Ordinal);
            var baseType = unsigned ? type.Substring(0, type.Length - " unsigned".Length) : type;
            if (baseType.StartsWith("enum(", StringComparison.Ordinal) || baseType.StartsWith("set(", StringComparison.Ordinal))
            {
                return field.DatabaseType;
            }

            switch (baseType)
            {
                case "char":
                case "binary":
                    return $"{baseType}({(field.Size > 0 ? field.Size.ToString() : "1")})";
                case "varchar":
                    return field.Size > 0 ? $"varchar({field.Size})" : "longtext";
                case "varbinary":
                    return field.Size > 0 ? $"varbinary({field.Size})" : "longblob";
                case "boolean":
                    return "tinyint(1)";
                case "uuid":
                    return "char(36)";
                case "bit":
                    return $"bit({(field.Precision > 0 ? field.Precision : (byte)1)})";
                case "decimal":
                    return $"decimal({(field.Precision > 0 ? field.Precision : (byte)18)},{field.Scale ?? 0})" + (unsigned ? " unsigned" : string.Empty);
                case "datetime":
                case "timestamp":
                case "time":
                    return field.Scale.HasValue && field.Scale.Value > 0 ? $"{baseType}({Math.Min(field.Scale.Value, (byte)6)})" : baseType;
                default:
                    return type;
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
            MariaDbSchemaHelper.Quote(name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string Name(string name) =>
            MariaDbSchemaHelper.QuoteName(name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string Literal(string value) =>
            value.Replace("\\", "\\\\").Replace("'", "''");

        /// <summary>
        /// Gets the filter of the table in the <c>information_schema</c>: the schema is the one of the name, or the current database.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="schemaColumn"></param>
        /// <param name="tableColumn"></param>
        /// <returns></returns>
        private static string TableFilter(string tableName,
            string schemaColumn,
            string tableColumn)
        {
            var (schema, table) = MariaDbSchemaHelper.Parse(tableName);
            return $"{schemaColumn} = {(schema == null ? "DATABASE()" : $"'{Literal(schema)}'")} AND {tableColumn} = '{Literal(table)}'";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        private static string TableName(TableSchema schema) =>
            MariaDbSchemaHelper.Format(schema.Table.Schema, schema.Table.Name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static string TableName(TableInfo table) =>
            MariaDbSchemaHelper.Format(table.Schema, table.Name);

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
                case CopySchemaForeignKeyRule.Restrict: return "RESTRICT";
                default: return null;
            }
        }

        // Columns

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private string Column(ColumnInfo column)
        {
            var field = column.Field;
            var definition = new StringBuilder(Quote(field.Name)).Append(' ');

            var type = ComposeTypeName(column);
            definition.Append(type);
            if (!string.IsNullOrWhiteSpace(column.Collation) && IsCharacterType(type))
            {
                definition.Append(" COLLATE ").Append(column.Collation);
            }

            // A generated column is always stored
            if (!string.IsNullOrWhiteSpace(column.ComputedExpression))
            {
                definition.Append(" GENERATED ALWAYS AS (").Append(column.ComputedExpression).Append(") STORED");
            }
            else
            {
                definition.Append(field.IsNullable ? " NULL" : " NOT NULL");
            }
            if (field.IsIdentity)
            {
                definition.Append(" AUTO_INCREMENT");
            }
            else if (string.IsNullOrWhiteSpace(column.ComputedExpression) && !string.IsNullOrWhiteSpace(column.DefaultExpression))
            {
                definition.Append(" DEFAULT ").Append(column.DefaultExpression);
            }
            if (!string.IsNullOrWhiteSpace(column.Comment))
            {
                definition.Append(" COMMENT '").Append(Literal(column.Comment)).Append('\'');
            }
            return definition.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsCharacterType(string type) =>
            type.StartsWith("char", StringComparison.Ordinal) ||
            type.StartsWith("varchar", StringComparison.Ordinal) ||
            type.EndsWith("text", StringComparison.Ordinal) ||
            type.StartsWith("enum(", StringComparison.Ordinal) ||
            type.StartsWith("set(", StringComparison.Ordinal);

        #endregion
    }
}
