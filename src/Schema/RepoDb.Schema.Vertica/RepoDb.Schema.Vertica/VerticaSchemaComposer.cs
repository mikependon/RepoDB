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
    /// An <see cref="ISchemaComposer"/> that composes the SQL statements, in the dialect of Vertica, that create the objects of a table.
    /// The column types are mapped on a best-effort basis, so the schema can come from a Vertica database or from another database engine.
    /// A statement has no terminator, and a DDL statement is committed by the server. Vertica has no index (it has projections), so no statement creates an index.
    /// </summary>
    public class VerticaSchemaComposer : ISchemaComposer
    {
        #region Private Variables

        private readonly DbTypeNameToVerticaTypeNameResolver _typeNameResolver = new DbTypeNameToVerticaTypeNameResolver();
        private readonly ClientTypeToVerticaTypeNameResolver _clientTypeResolver = new ClientTypeToVerticaTypeNameResolver();

        #endregion

        #region Public Methods

        /// <summary>
        /// Composes the whole script that creates the table and its foreign keys, as an ordered list of statements.
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
            statements.AddRange(schema.ForeignKeys.Select(foreignKey => ComposeAddForeignKey(tableName, foreignKey)));
            return statements;
        }

        /// <summary>
        /// Composes the whole script that creates the tables and their foreign keys, as an ordered list of statements.
        /// All the tables are created first, then all the foreign keys, so the tables can be given in any order
        /// and can reference each other (even in a circular way) as long as the referenced tables are part of the given tables
        /// or already exist in the destination database.
        /// The statements are ordered like this: one statement for each table (in the given order), then one statement for each foreign key (the tables in the given order).
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
                definitions.Add($"{ConstraintName(schema.PrimaryKey.Name)}PRIMARY KEY ({Columns(schema.PrimaryKey.Columns)})");
            }
            definitions.AddRange(schema.UniqueConstraints.Select(u =>
                $"{ConstraintName(u.Name)}UNIQUE ({Columns(u.Columns)})"));
            definitions.AddRange(schema.CheckConstraints.Select(c =>
                $"{ConstraintName(c.Name)}CHECK ({c.Expression})"));

            return $"CREATE TABLE {Name(TableName(schema))} ({Environment.NewLine}    {string.Join("," + Environment.NewLine + "    ", definitions)}{Environment.NewLine})";
        }

        /// <summary>
        /// Vertica does not support indexes (it has projections), so the statement can not be composed.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="index">The index to be created.</param>
        /// <returns>The SQL statement.</returns>
        /// <exception cref="NotSupportedException">Always, as Vertica has no index.</exception>
        public string ComposeCreateIndex(string tableName,
            IndexInfo index)
        {
            if (index == null)
            {
                throw new ArgumentNullException(nameof(index));
            }
            throw new NotSupportedException("Vertica does not support indexes (it has projections).");
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
            return statement.ToString();
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
        /// Composes the statement that drops the table (with the foreign keys that reference it), if it exists.
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
            VerticaSchemaHelper.FormatTableName((table ?? throw new ArgumentNullException(nameof(table))).Schema, table.Name);

        /// <summary>
        /// Composes the statement that checks whether the table exists in the destination database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The SQL statement, that returns <c>1</c> if the table exists, and <c>0</c> if not.</returns>
        public string ComposeTableExists(string tableName)
        {
            var (schema, table) = VerticaSchemaHelper.ParseSchemaAndTable(tableName);
            return $"SELECT CASE WHEN EXISTS (SELECT 1 FROM v_catalog.tables WHERE {SchemaPredicate(schema)} AND table_name = '{Literal(table)}') THEN 1 ELSE 0 END";
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
            var (schema, table) = VerticaSchemaHelper.ParseSchemaAndTable(tableName);
            return $"SELECT CASE WHEN EXISTS (SELECT 1 FROM v_catalog.columns WHERE {SchemaPredicate(schema)} AND table_name = '{Literal(table)}' AND column_name = '{Literal(columnName)}') THEN 1 ELSE 0 END";
        }

        /// <summary>
        /// Composes the statement that checks whether an index exists in a table of the destination database. Vertica has no index, so the index never exists.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="indexName">The name of the index.</param>
        /// <returns>The SQL statement, that returns <c>1</c> if the index exists, and <c>0</c> if not.</returns>
        public string ComposeIndexExists(string tableName,
            string indexName)
        {
            VerticaSchemaHelper.ParseSchemaAndTable(tableName);
            return "SELECT 0";
        }

        /// <summary>
        /// Composes the data type of a column (including its size, precision and scale) in the dialect of the destination database.
        /// A column that comes from Vertica keeps the type that it declares.
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
            var isVertica = string.Equals(field.Provider, "Vertica", StringComparison.Ordinal);
            var type = isVertica ? field.DatabaseType.ToLowerInvariant() : _typeNameResolver.Resolve(field.DatabaseType) ?? _clientTypeResolver.Resolve(field.Type);

            switch (type)
            {
                case "varchar":
                case "character varying":
                    return ComposeVariableType("VARCHAR", "LONG VARCHAR", 65000, field.Size, isVertica);
                case "long varchar":
                    return field.Size > 0 && field.Size <= 32000000 ? $"LONG VARCHAR({field.Size})" : "LONG VARCHAR";
                case "char":
                case "character":
                    return field.Size > 0 && field.Size <= 65000 ? $"CHAR({field.Size})" : isVertica ? "CHAR" : "CHAR(1)";
                case "binary":
                    return field.Size > 0 && field.Size <= 65000 ? $"BINARY({field.Size})" : isVertica ? "BINARY" : "LONG VARBINARY";
                case "varbinary":
                    return ComposeVariableType("VARBINARY", "LONG VARBINARY", 65000, field.Size, isVertica);
                case "long varbinary":
                    return field.Size > 0 && field.Size <= 32000000 ? $"LONG VARBINARY({field.Size})" : "LONG VARBINARY";
                case "numeric":
                case "decimal":
                    return field.Precision > 0 ? $"NUMERIC({Math.Min((int)field.Precision, 1024)},{Math.Min((int)(field.Scale ?? 0), (int)field.Precision)})" : "NUMERIC";
                case "timestamp":
                    return ComposeTemporalType("TIMESTAMP", field);
                case "timestamp with timezone":
                case "timestamptz":
                    return ComposeTemporalType("TIMESTAMPTZ", field);
                case "time":
                    return ComposeTemporalType("TIME", field);
                case "time with timezone":
                case "timetz":
                    return "TIMETZ";
                default:
                    return type.ToUpperInvariant();
            }
        }

        #endregion

        #region Helpers

        // Types

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="largeType"></param>
        /// <param name="limit"></param>
        /// <param name="size"></param>
        /// <param name="isVertica"></param>
        /// <returns></returns>
        private static string ComposeVariableType(string type,
            string largeType,
            int limit,
            int? size,
            bool isVertica)
        {
            if (size > 0 && size <= limit)
            {
                return $"{type}({size})";
            }
            return isVertica && size > 0 ? $"{type}({limit})" : largeType;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        private static string ComposeTemporalType(string type,
            DbField field) =>
            field.Scale > 0 ? $"{type}({Math.Min((int)field.Scale.Value, 6)})" : type;

        // Names

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string Quote(string name) =>
            VerticaSchemaHelper.Quote(name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string Name(string name) =>
            VerticaSchemaHelper.QuoteName(name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string Literal(string value) =>
            value.Replace("'", "''");

        /// <summary>
        /// Gets the condition of the schema of the catalog. A table without a schema is in the schema of the user if it exists, or in the <c>public</c> schema otherwise
        /// (the default search path of Vertica is <c>"$user", public</c>), as the <c>CURRENT_SCHEMA()</c> meta-function of Vertica can not be used in a condition.
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        private static string SchemaPredicate(string schema) =>
            schema == null
                ? "table_schema = (CASE WHEN EXISTS (SELECT 1 FROM v_catalog.schemata WHERE schema_name = SESSION_USER()) THEN SESSION_USER() ELSE 'public' END)"
                : $"table_schema = '{Literal(schema)}'";

        /// <summary>
        ///
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        private static string TableName(TableSchema schema) =>
            VerticaSchemaHelper.FormatTableName(schema.Table.Schema, schema.Table.Name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static string TableName(TableInfo table) =>
            VerticaSchemaHelper.FormatTableName(table.Schema, table.Name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        private static string Columns(IEnumerable<string> columns) =>
            string.Join(", ", columns.Select(Quote));

        /// <summary>
        /// The names that Vertica generates for the unnamed constraints (<c>C_PRIMARY</c>, <c>C_UNIQUE</c>, <c>C_FOREIGN</c> and <c>C_CHECK</c>) are not given back, so the constraint is unnamed again.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string ConstraintName(string name) =>
            string.IsNullOrWhiteSpace(name) || System.Text.RegularExpressions.Regex.IsMatch(name, @"^C_(PRIMARY|UNIQUE|FOREIGN|CHECK)$") ? string.Empty : $"CONSTRAINT {Quote(name)} ";

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
            if (field.IsIdentity)
            {
                return new StringBuilder(Quote(field.Name))
                    .Append(" IDENTITY(").Append(column.IdentitySeed ?? 1).Append(", ").Append(column.IdentityIncrement ?? 1).Append(") NOT NULL")
                    .ToString();
            }
            if (!string.IsNullOrWhiteSpace(column.ComputedExpression))
            {
                definition.Append(" DEFAULT (").Append(column.ComputedExpression).Append(')');
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
