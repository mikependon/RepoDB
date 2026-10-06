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
    /// An <see cref="ISchemaComposer"/> that composes the SQL statements, in the dialect of Db2, that create the objects of a table.
    /// The column types are mapped on a best-effort basis, so the schema can come from a Db2 database or from another database engine.
    /// A statement has no terminator, and a DDL statement is part of the transaction (it can be rolled back).
    /// </summary>
    public class Db2SchemaComposer : ISchemaComposer
    {
        #region Private Variables

        private const string CurrentSchema = "CURRENT SCHEMA";
        private readonly DbTypeNameToDb2TypeNameResolver _typeNameResolver = new DbTypeNameToDb2TypeNameResolver();
        private readonly ClientTypeToDb2TypeNameResolver _clientTypeResolver = new ClientTypeToDb2TypeNameResolver();

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
                definitions.Add($"{ConstraintName(schema.PrimaryKey.Name)}PRIMARY KEY ({Columns(schema.PrimaryKey.Columns)})");
            }
            definitions.AddRange(schema.UniqueConstraints.Select(u =>
                $"{ConstraintName(u.Name)}UNIQUE ({Columns(u.Columns)})"));
            definitions.AddRange(schema.CheckConstraints.Select(c =>
                $"{ConstraintName(c.Name)}CHECK ({c.Expression})"));

            return $"CREATE TABLE {Name(TableName(schema))} ({Environment.NewLine}    {string.Join("," + Environment.NewLine + "    ", definitions)}{Environment.NewLine})";
        }

        /// <summary>
        /// Composes the statement that creates an index. The index is created in the schema of the table. The included columns are supported only by the unique indexes.
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

            var (schema, _) = Db2SchemaHelper.ParseSchemaAndTable(tableName);
            var statement = new StringBuilder("CREATE ");
            if (index.IsUnique)
            {
                statement.Append("UNIQUE ");
            }
            var keys = string.Join(", ", index.Columns.Select(column =>
                index.DescendingColumns != null && index.DescendingColumns.Contains(column, StringComparer.Ordinal)
                    ? $"{Quote(column)} DESC"
                    : Quote(column)));
            var indexName = schema == null ? Quote(index.Name) : Db2SchemaHelper.QuoteSchemaAndTable(schema, index.Name);
            statement.Append($"INDEX {indexName} ON {Name(tableName)} ({keys})");
            if (index.IsUnique && index.IncludedColumns != null && index.IncludedColumns.Count > 0)
            {
                statement.Append($" INCLUDE ({Columns(index.IncludedColumns)})");
            }
            return statement.ToString();
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
            var onDelete = Rule(foreignKey.DeleteRule, true);
            if (onDelete != null)
            {
                statement.Append($" ON DELETE {onDelete}");
            }
            var onUpdate = Rule(foreignKey.UpdateRule, false);
            if (onUpdate != null)
            {
                statement.Append($" ON UPDATE {onUpdate}");
            }
            return statement.ToString();
        }

        /// <summary>
        /// Composes the statement that adds a column to an existing table. Db2 adds a generated column only while the integrity of the table is off,
        /// so the statement of a generated column is a script of 3 statements that are separated by a semicolon.
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
            var statement = $"ALTER TABLE {Name(tableName)} ADD COLUMN {Column(column)}";
            if (string.IsNullOrWhiteSpace(column.ComputedExpression))
            {
                return statement;
            }
            return $"SET INTEGRITY FOR {Name(tableName)} OFF; {statement}; SET INTEGRITY FOR {Name(tableName)} IMMEDIATE CHECKED FORCE GENERATED";
        }

        /// <summary>
        /// Composes the statement that drops the table (with its indexes and constraints).
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The SQL statement.</returns>
        public string ComposeDropTable(string tableName) =>
            $"DROP TABLE IF EXISTS {Name(tableName)}";

        /// <summary>
        /// Composes the name of the table, in the dialect of the destination database, so it can be given to the other methods that take the name of a table.
        /// </summary>
        /// <param name="table">The identity (name and schema) of the table.</param>
        /// <returns>The name of the table.</returns>
        public string ComposeName(TableInfo table) =>
            Db2SchemaHelper.FormatTableName((table ?? throw new ArgumentNullException(nameof(table))).Schema, table.Name);

        /// <summary>
        /// Composes the statement that checks whether the table exists in the destination database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The SQL statement, that returns <c>1</c> if the table exists, and <c>0</c> if not.</returns>
        public string ComposeTableExists(string tableName)
        {
            var (schema, table) = Db2SchemaHelper.ParseSchemaAndTable(tableName);
            return $"SELECT CASE WHEN EXISTS (SELECT 1 FROM SYSCAT.TABLES WHERE TABSCHEMA = {Owner(schema)} AND TABNAME = '{Literal(table)}') THEN 1 ELSE 0 END FROM SYSIBM.SYSDUMMY1";
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
            var (schema, table) = Db2SchemaHelper.ParseSchemaAndTable(tableName);
            return $"SELECT CASE WHEN EXISTS (SELECT 1 FROM SYSCAT.COLUMNS WHERE TABSCHEMA = {Owner(schema)} AND TABNAME = '{Literal(table)}' AND COLNAME = '{Literal(columnName)}') THEN 1 ELSE 0 END FROM SYSIBM.SYSDUMMY1";
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
            var (schema, table) = Db2SchemaHelper.ParseSchemaAndTable(tableName);
            return $"SELECT CASE WHEN EXISTS (SELECT 1 FROM SYSCAT.INDEXES WHERE TABSCHEMA = {Owner(schema)} AND TABNAME = '{Literal(table)}' AND INDNAME = '{Literal(indexName)}') THEN 1 ELSE 0 END FROM SYSIBM.SYSDUMMY1";
        }

        /// <summary>
        /// Composes the data type of a column (including its size, precision and scale) in the dialect of the destination database.
        /// A column that comes from Db2 keeps the type that it declares.
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
            var isDb2 = string.Equals(field.Provider, "Db2", StringComparison.Ordinal);
            var type = isDb2 ? field.DatabaseType.ToLowerInvariant() : _typeNameResolver.Resolve(field.DatabaseType) ?? _clientTypeResolver.Resolve(field.Type);

            switch (type)
            {
                case "varchar":
                case "character varying":
                    return ComposeVariableCharacterType("VARCHAR", "CLOB", 32672, field.Size, isDb2);
                case "vargraphic":
                    return ComposeVariableCharacterType("VARGRAPHIC", "DBCLOB", 16336, field.Size, isDb2);
                case "char":
                case "character":
                    return ComposeFixedCharacterType("CHAR", "VARCHAR", field.Size, isDb2);
                case "graphic":
                    return ComposeFixedCharacterType("GRAPHIC", "VARGRAPHIC", field.Size, isDb2);
                case "binary":
                    return field.Size > 0 && field.Size <= 255 ? $"BINARY({field.Size})" : isDb2 ? "BINARY" : "BLOB";
                case "varbinary":
                    return ComposeVariableCharacterType("VARBINARY", "BLOB", 32672, field.Size, isDb2);
                case "decimal":
                    return ComposeDecimalType(field);
                case "decfloat":
                    return field.Precision == 16 ? "DECFLOAT(16)" : "DECFLOAT(34)";
                case "timestamp":
                    return $"TIMESTAMP({(field.Scale.HasValue ? Math.Min(field.Scale.Value, (byte)12) : 6)})";
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
        /// <param name="isDb2"></param>
        /// <returns></returns>
        private static string ComposeVariableCharacterType(string type,
            string largeType,
            int limit,
            int? size,
            bool isDb2)
        {
            if (size > 0 && size <= limit)
            {
                return $"{type}({size})";
            }
            return isDb2 && size > 0 ? type + "(" + limit + ")" : largeType;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="variableType"></param>
        /// <param name="size"></param>
        /// <param name="isDb2"></param>
        /// <returns></returns>
        private static string ComposeFixedCharacterType(string type,
            string variableType,
            int? size,
            bool isDb2)
        {
            if (size > 0 && size <= 255)
            {
                return $"{type}({size})";
            }
            return size > 255 ? $"{variableType}({size})" : isDb2 ? type : $"{type}(1)";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private static string ComposeDecimalType(DbField field)
        {
            if (field.Precision > 0)
            {
                var precision = Math.Min((int)field.Precision, 31);
                return $"DECIMAL({precision},{Math.Min((int)(field.Scale ?? 0), precision)})";
            }
            return "DECIMAL(31,6)";
        }

        // Names

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string Quote(string name) =>
            Db2SchemaHelper.Quote(name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string Name(string name) =>
            Db2SchemaHelper.QuoteName(name);

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
            Db2SchemaHelper.FormatTableName(schema.Table.Schema, schema.Table.Name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static string TableName(TableInfo table) =>
            Db2SchemaHelper.FormatTableName(table.Schema, table.Name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        private static string Columns(IEnumerable<string> columns) =>
            string.Join(", ", columns.Select(Quote));

        /// <summary>
        /// The names that Db2 generates for the unnamed constraints (<c>SQL</c> followed by a timestamp) are not given back, so the constraint is unnamed again.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string ConstraintName(string name) =>
            string.IsNullOrWhiteSpace(name) || System.Text.RegularExpressions.Regex.IsMatch(name, @"^SQL\d{10,}$") ? string.Empty : $"CONSTRAINT {Quote(name)} ";

        /// <summary>
        ///
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        private static string Rule(CopySchemaForeignKeyRule rule,
            bool isDelete)
        {
            switch (rule)
            {
                case CopySchemaForeignKeyRule.Cascade: return isDelete ? "CASCADE" : null;
                case CopySchemaForeignKeyRule.SetNull: return isDelete ? "SET NULL" : null;
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
            var definition = new StringBuilder(Quote(field.Name)).Append(' ').Append(ComposeTypeName(column));
            if (!string.IsNullOrWhiteSpace(column.ComputedExpression))
            {
                return definition.Append(" GENERATED ALWAYS AS (").Append(column.ComputedExpression).Append(')').ToString();
            }
            if (!field.IsNullable)
            {
                definition.Append(" NOT NULL");
            }
            if (field.IsIdentity)
            {
                definition.Append(" GENERATED BY DEFAULT AS IDENTITY (START WITH ").Append(column.IdentitySeed ?? 1).Append(" INCREMENT BY ").Append(column.IdentityIncrement ?? 1).Append(')');
            }
            else if (!string.IsNullOrWhiteSpace(column.DefaultExpression))
            {
                definition.Append(" WITH DEFAULT ").Append(column.DefaultExpression);
            }
            return definition.ToString();
        }

        #endregion
    }
}
