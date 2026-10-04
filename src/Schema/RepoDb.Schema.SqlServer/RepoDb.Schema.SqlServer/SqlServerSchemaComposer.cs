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
    /// An <see cref="ISchemaComposer"/> that composes the T-SQL statements, in the dialect of SQL Server, that create the objects of a table.
    /// The column types are mapped on a best-effort basis, so the schema can come from a SQL Server database or from another database engine.
    /// </summary>
    public class SqlServerSchemaComposer : ISchemaComposer
    {
        #region Private Variables

        private readonly DbTypeNameToSqlServerTypeNameResolver _typeNameResolver = new DbTypeNameToSqlServerTypeNameResolver();
        private readonly ClientTypeToSqlServerTypeNameResolver _clientTypeResolver = new ClientTypeToSqlServerTypeNameResolver();

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
                definitions.Add($"{ConstraintName(schema.PrimaryKey.Name)}PRIMARY KEY {(schema.PrimaryKey.IsClustered ? string.Empty : "NONCLUSTERED ")}({Columns(schema.PrimaryKey.Columns)})");
            }
            definitions.AddRange(schema.UniqueConstraints.Select(u =>
                $"{ConstraintName(u.Name)}UNIQUE ({Columns(u.Columns)})"));
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
            if (index.IsClustered)
            {
                statement.Append("CLUSTERED ");
            }

            // The key columns, each one with its sort order
            var keys = string.Join(", ", index.Columns.Select(column =>
                index.DescendingColumns != null && index.DescendingColumns.Contains(column, StringComparer.Ordinal)
                    ? $"{Quote(column)} DESC"
                    : Quote(column)));
            statement.Append($"INDEX {Quote(index.Name)} ON {Name(tableName)} ({keys})");
            if (index.IncludedColumns != null && index.IncludedColumns.Count > 0)
            {
                statement.Append($" INCLUDE ({Columns(index.IncludedColumns)})");
            }
            if (!string.IsNullOrWhiteSpace(index.Filter))
            {
                statement.Append($" WHERE {index.Filter}");
            }
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
                $"REFERENCES {Name(foreignKey.ReferencedTable)} ({Columns(foreignKey.ReferencedColumns)})");

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
            return $"ALTER TABLE {Name(tableName)} ADD {Column(column)};";
        }

        /// <summary>
        /// Composes the statement that drops the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The SQL statement.</returns>
        public string ComposeDropTable(string tableName) =>
            $"DROP TABLE IF EXISTS {Name(tableName)};";

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

            switch (type)
            {
                case "char":
                case "nchar":
                case "binary":
                    return $"{type}({(field.Size > 0 ? field.Size.ToString() : "1")})";
                case "varchar":
                case "nvarchar":
                case "varbinary":
                    return $"{type}({(field.Size > 0 ? field.Size.ToString() : "MAX")})";
                case "decimal":
                case "numeric":
                    return $"{type}({(field.Precision > 0 ? field.Precision : (byte)18)},{field.Scale ?? 0})";
                case "datetime2":
                case "datetimeoffset":
                case "time":
                    return field.Scale.HasValue ? $"{type}({field.Scale})" : type;
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
            Helper.Quote(name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string Name(string name) =>
            Helper.QuoteName(name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        private static string TableName(TableSchema schema) =>
            Helper.Format(schema.SchemaName, schema.TableName);

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
        private static string Rule(ForeignKeyRule rule)
        {
            switch (rule)
            {
                case ForeignKeyRule.Cascade: return "CASCADE";
                case ForeignKeyRule.SetNull: return "SET NULL";
                case ForeignKeyRule.SetDefault: return "SET DEFAULT";

                // SQL Server has no RESTRICT; NO ACTION is its (default) equivalent
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

            // A computed column has no type
            if (!string.IsNullOrWhiteSpace(column.ComputedExpression))
            {
                return definition.Append("AS ").Append(column.ComputedExpression).ToString();
            }

            var type = ComposeTypeName(column);
            definition.Append(type);
            if (!string.IsNullOrWhiteSpace(column.Collation) && IsCharacterType(type))
            {
                definition.Append(" COLLATE ").Append(column.Collation);
            }
            if (field.IsIdentity)
            {
                definition.Append($" IDENTITY({column.IdentitySeed ?? 1},{column.IdentityIncrement ?? 1})");
            }
            definition.Append(field.IsNullable ? " NULL" : " NOT NULL");
            if (!field.IsIdentity && !string.IsNullOrWhiteSpace(column.DefaultExpression))
            {
                definition.Append(" DEFAULT ").Append(column.DefaultExpression);
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
            type.StartsWith("nchar", StringComparison.Ordinal) ||
            type.StartsWith("nvarchar", StringComparison.Ordinal);

        #endregion
    }
}
