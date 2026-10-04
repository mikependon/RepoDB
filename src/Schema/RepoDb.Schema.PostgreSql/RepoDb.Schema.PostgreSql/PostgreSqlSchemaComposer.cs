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
    /// An <see cref="ISchemaComposer"/> that composes the SQL statements, in the dialect of PostgreSQL, that create the objects of a table.
    /// The column types are mapped on a best-effort basis, so the schema can come from a PostgreSQL database or from another database engine.
    /// The collations are not composed, as they are specific to the database that they came from.
    /// </summary>
    public class PostgreSqlSchemaComposer : ISchemaComposer
    {
        #region Private Variables

        private readonly DbTypeNameToPostgreSqlTypeNameResolver _typeNameResolver = new DbTypeNameToPostgreSqlTypeNameResolver();
        private readonly ClientTypeToPostgreSqlTypeNameResolver _clientTypeResolver = new ClientTypeToPostgreSqlTypeNameResolver();

        #endregion

        #region Public Methods

        /// <summary>
        /// Composes the whole script that creates the table, its indexes and its foreign keys, as an ordered list of statements.
        /// </summary>
        /// <param name="schema">The schema of the table.</param>
        /// <returns>The ordered SQL statements. Execute them in order.</returns>
        public IEnumerable<string> ComposeSchema(TableSchema schema) =>
            ComposeSchemas(new[] { schema ?? throw new ArgumentNullException(nameof(schema)) });

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
            statements.AddRange(list.SelectMany(schema => schema.Indexes.Select(index => ComposeCreateIndex(TableName(schema), index))));
            statements.AddRange(list.SelectMany(schema => schema.ForeignKeys.Select(foreignKey => ComposeAddForeignKey(TableName(schema), foreignKey))));
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
            if (schema.Columns.Count == 0)
            {
                // PostgreSQL accepts a table without columns, so a table that does not exist in the source would be silently created as an empty one
                throw new ArgumentException("The table has no columns, so it does not exist or it cannot be created.", nameof(schema));
            }

            var definitions = new List<string>();
            definitions.AddRange(schema.Columns.OrderBy(c => c.Ordinal).Select(Column));

            if (schema.PrimaryKey != null && schema.PrimaryKey.Columns.Count > 0)
            {
                definitions.Add($"{ConstraintName(schema.PrimaryKey.Name)}PRIMARY KEY ({Columns(schema.PrimaryKey.Columns)})");
            }
            definitions.AddRange(schema.UniqueConstraints.Select(u => $"{ConstraintName(u.Name)}UNIQUE ({Columns(u.Columns)})"));
            definitions.AddRange(schema.CheckConstraints.Select(c => $"{ConstraintName(c.Name)}CHECK ({c.Expression})"));

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

            // The key columns, each one with its sort order (PostgreSQL has no clustered index)
            var keys = string.Join(", ", index.Columns.Select(column =>
                index.DescendingColumns != null && index.DescendingColumns.Contains(column, StringComparer.Ordinal)
                    ? $"{Quote(column)} DESC"
                    : Quote(column)));
            var statement = new StringBuilder($"CREATE {(index.IsUnique ? "UNIQUE " : string.Empty)}INDEX {Quote(index.Name)} ON {Name(tableName)} ({keys})");
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

            return $"ALTER TABLE {Name(tableName)} ADD {ConstraintName(foreignKey.Name)}FOREIGN KEY ({Columns(foreignKey.Columns)}) " +
                $"REFERENCES {Name(foreignKey.ReferencedTable)} ({Columns(foreignKey.ReferencedColumns)})" +
                $"{Rule("DELETE", foreignKey.DeleteRule)}{Rule("UPDATE", foreignKey.UpdateRule)};";
        }

        /// <summary>
        /// Composes the statement that adds a column to an existing table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="column">The column to be added.</param>
        /// <returns>The SQL statement.</returns>
        public string ComposeAddColumn(string tableName,
            ColumnInfo column) =>
            $"ALTER TABLE {Name(tableName)} ADD COLUMN {Column(column ?? throw new ArgumentNullException(nameof(column)))};";

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
                    return $"char({(field.Size > 0 ? field.Size.ToString() : "1")})";
                case "varchar":
                    return field.Size > 0 ? $"varchar({field.Size})" : type;
                case "numeric":
                    return field.Precision > 0 ? $"numeric({field.Precision},{field.Scale ?? 0})" : type;
                case "timestamp":
                case "timestamptz":
                case "time":
                case "timetz":
                    return field.Scale.HasValue ? $"{type}({Math.Min(field.Scale.Value, (byte)6)})" : type;
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
        /// <param name="action"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        private static string Rule(string action, ForeignKeyRule rule)
        {
            switch (rule)
            {
                case ForeignKeyRule.Cascade: return $" ON {action} CASCADE";
                case ForeignKeyRule.SetNull: return $" ON {action} SET NULL";
                case ForeignKeyRule.SetDefault: return $" ON {action} SET DEFAULT";
                case ForeignKeyRule.Restrict: return $" ON {action} RESTRICT";
                default: return string.Empty;
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
            var definition = new StringBuilder(Quote(field.Name)).Append(' ').Append(ComposeTypeName(column));

            // A computed column is stored, and it is always generated from its expression
            if (!string.IsNullOrWhiteSpace(column.ComputedExpression))
            {
                return definition.Append($" GENERATED ALWAYS AS ({column.ComputedExpression}) STORED").ToString();
            }
            if (field.IsIdentity)
            {
                definition.Append($" GENERATED BY DEFAULT AS IDENTITY (START WITH {column.IdentitySeed ?? 1} INCREMENT BY {column.IdentityIncrement ?? 1})");
            }
            definition.Append(field.IsNullable ? " NULL" : " NOT NULL");
            if (!field.IsIdentity && !string.IsNullOrWhiteSpace(column.DefaultExpression))
            {
                definition.Append(" DEFAULT ").Append(column.DefaultExpression);
            }
            return definition.ToString();
        }

        #endregion
    }
}
