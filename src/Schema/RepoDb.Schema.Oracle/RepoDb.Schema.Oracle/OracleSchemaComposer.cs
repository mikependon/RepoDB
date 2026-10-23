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
    /// An <see cref="ISchemaComposer"/> that composes the SQL statements, in the dialect of Oracle, that create the objects of a table.
    /// The column types are mapped on a best-effort basis, so the schema can come from an Oracle database or from another database engine.
    /// A statement has no terminator (Oracle does not accept the <c>;</c> from the client), and a DDL statement is committed by the server.
    /// </summary>
    public class OracleSchemaComposer : ISchemaComposer, ICheckConstraintSupport
    {
        #region Private Variables

        private const string CurrentSchema = "SYS_CONTEXT('USERENV', 'CURRENT_SCHEMA')";
        private readonly DbTypeNameToOracleTypeNameResolver _typeNameResolver = new DbTypeNameToOracleTypeNameResolver();
        private readonly ClientTypeToOracleTypeNameResolver _clientTypeResolver = new ClientTypeToOracleTypeNameResolver();

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the reason why Oracle refuses the check constraint: a function that is not deterministic (like the current date and time) can not be used in a <c>CHECK</c>.
        /// </summary>
        /// <param name="checkConstraint">The check constraint of the source table.</param>
        /// <returns>The reason, or <c>null</c> if the check constraint can be created.</returns>
        public string GetUnsupportedReason(CheckConstraintInfo checkConstraint) =>
            SchemaExpression.GetNonDeterministicReason(checkConstraint, "Oracle");

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
        /// Composes the statement that creates an index. The index is created in the schema of the table.
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

            var (schema, _) = OracleSchemaHelper.ParseSchemaAndTable(tableName);
            var statement = new StringBuilder("CREATE ");
            if (index.IsUnique)
            {
                statement.Append("UNIQUE ");
            }
            var keys = string.Join(", ", index.Columns.Select(column =>
                index.DescendingColumns != null && index.DescendingColumns.Contains(column, StringComparer.Ordinal)
                    ? $"{Quote(column)} DESC"
                    : Quote(column)));
            var indexName = schema == null ? Quote(index.Name) : OracleSchemaHelper.QuoteSchemaAndTable(schema, index.Name);
            return statement.Append($"INDEX {indexName} ON {Name(tableName)} ({keys})").ToString();
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
            return $"ALTER TABLE {Name(tableName)} ADD ({Column(column)})";
        }

        /// <summary>
        /// Composes the statement that drops the table (with its constraints, and without keeping it in the recycle bin).
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The SQL statement.</returns>
        public string ComposeDropTable(string tableName) =>
            $"DROP TABLE IF EXISTS {Name(tableName)} CASCADE CONSTRAINTS PURGE";

        /// <summary>
        /// Composes the name of the table, in the dialect of the destination database, so it can be given to the other methods that take the name of a table.
        /// </summary>
        /// <param name="table">The identity (name and schema) of the table.</param>
        /// <returns>The name of the table.</returns>
        public string ComposeName(TableInfo table) =>
            OracleSchemaHelper.FormatTableName((table ?? throw new ArgumentNullException(nameof(table))).Schema, table.Name);

        /// <summary>
        /// Composes the statement that checks whether the table exists in the destination database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The SQL statement, that returns <c>1</c> if the table exists, and <c>0</c> if not.</returns>
        public string ComposeTableExists(string tableName)
        {
            var (schema, table) = OracleSchemaHelper.ParseSchemaAndTable(tableName);
            return $"SELECT CASE WHEN EXISTS (SELECT 1 FROM ALL_TABLES WHERE OWNER = {Owner(schema)} AND TABLE_NAME = '{Literal(table)}') THEN 1 ELSE 0 END FROM DUAL";
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
            var (schema, table) = OracleSchemaHelper.ParseSchemaAndTable(tableName);
            return $"SELECT CASE WHEN EXISTS (SELECT 1 FROM ALL_TAB_COLS WHERE OWNER = {Owner(schema)} AND TABLE_NAME = '{Literal(table)}' AND COLUMN_NAME = '{Literal(columnName)}') THEN 1 ELSE 0 END FROM DUAL";
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
            var (schema, table) = OracleSchemaHelper.ParseSchemaAndTable(tableName);
            return $"SELECT CASE WHEN EXISTS (SELECT 1 FROM ALL_INDEXES WHERE TABLE_OWNER = {Owner(schema)} AND TABLE_NAME = '{Literal(table)}' AND INDEX_NAME = '{Literal(indexName)}') THEN 1 ELSE 0 END FROM DUAL";
        }

        /// <summary>
        /// Composes the statement that checks whether the name of an index is already used in the destination database, by any table of the schema of the given table.
        /// </summary>
        /// <param name="tableName">The name of the table that the index is going to be created in.</param>
        /// <param name="indexName">The name of the index.</param>
        /// <returns>The SQL statement, that returns <c>1</c> if the name is used, and <c>0</c> if not (<c>null</c> if the setting of the provider says that the names of the indexes are only unique in their table).</returns>
        public string ComposeIndexNameExists(string tableName,
            string indexName)
        {
            if (!OracleSchemaHelper.AreIndexNamesUniquePerSchema)
            {
                return null;
            }

            var (schema, _) = OracleSchemaHelper.ParseSchemaAndTable(tableName);
            return $"SELECT CASE WHEN EXISTS (SELECT 1 FROM ALL_INDEXES WHERE OWNER = {Owner(schema)} AND INDEX_NAME = '{Literal(indexName)}') THEN 1 ELSE 0 END FROM DUAL";
        }

        /// <summary>
        /// Composes the statement that checks whether a foreign key exists in a table of the destination database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="foreignKeyName">The name of the foreign key.</param>
        /// <returns>The SQL statement, that returns <c>1</c> if the foreign key exists, and <c>0</c> if not.</returns>
        public string ComposeForeignKeyExists(string tableName,
            string foreignKeyName)
        {
            var (schema, table) = OracleSchemaHelper.ParseSchemaAndTable(tableName);
            return $"SELECT CASE WHEN EXISTS (SELECT 1 FROM ALL_CONSTRAINTS WHERE CONSTRAINT_TYPE = 'R' AND OWNER = {Owner(schema)} AND TABLE_NAME = '{Literal(table)}' AND CONSTRAINT_NAME = '{Literal(foreignKeyName)}') THEN 1 ELSE 0 END FROM DUAL";
        }

        /// <summary>
        /// Composes the data type of a column (including its size, precision and scale) in the dialect of the destination database.
        /// A column that comes from Oracle keeps the type that it declares.
        /// </summary>
        /// <param name="column">The column (as read from the source database) to be mapped.</param>
        /// <returns>The destination data type, i.e.: <c>VARCHAR2(128)</c>.</returns>
        public string ComposeTypeName(ColumnInfo column)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }

            var field = column.Field ?? throw new ArgumentException("The column has no field.", nameof(column));
            var isOracle = string.Equals(field.Provider, "Oracle", StringComparison.Ordinal);
            var type = isOracle ? field.DatabaseType.ToLowerInvariant() : _typeNameResolver.Resolve(field.DatabaseType) ?? _clientTypeResolver.Resolve(field.Type);

            switch (type)
            {
                case "varchar2":
                case "nvarchar2":
                case "char":
                case "nchar":
                    return ComposeCharacterType(type, field.Size, isOracle);
                case "raw":
                    return field.Size > 0 && field.Size <= 2000 ? $"RAW({field.Size})" : isOracle ? "RAW(2000)" : "BLOB";
                case "number":
                    return ComposeNumberType(field);
                case "interval year to month":
                    return field.Precision > 0 ? $"INTERVAL YEAR({field.Precision}) TO MONTH" : "INTERVAL YEAR TO MONTH";
                case "interval day to second":
                    return field.Precision > 0 ? $"INTERVAL DAY({field.Precision}) TO SECOND({field.Scale ?? 6})" : "INTERVAL DAY TO SECOND";
                case "timestamp":
                case "timestamp with time zone":
                case "timestamp with local time zone":
                    return ComposeTimestampType(type, field);
                default:
                    return type.ToUpperInvariant();
            }
        }

        #endregion

        #region Helpers

        // Types

        /// <summary>
        /// Gets the character type with its size. A type that is too large for a sized type is composed as the large one.
        /// </summary>
        /// <param name="type">The name of the type.</param>
        /// <param name="size">The size of the column.</param>
        /// <param name="isOracle">A value that indicates whether the destination database is Oracle.</param>
        /// <returns>The name of the type.</returns>
        private static string ComposeCharacterType(string type,
            int? size,
            bool isOracle)
        {
            var limit = type == "char" || type == "nchar" ? 2000 : 4000;
            if (size > 0 && size <= limit)
            {
                return $"{type.ToUpperInvariant()}({size})";
            }
            if (isOracle)
            {
                return type.ToUpperInvariant();
            }
            if (type == "char" || type == "nchar")
            {
                return $"{type.ToUpperInvariant()}(1)";
            }
            return type == "nvarchar2" ? "NCLOB" : "CLOB";
        }

        /// <summary>
        /// Gets the number type with the precision and the scale of the field.
        /// </summary>
        /// <param name="field">The field of the column.</param>
        /// <returns>The name of the type.</returns>
        private static string ComposeNumberType(DbField field)
        {
            if (field.Precision > 0)
            {
                return field.Scale > 0 ? $"NUMBER({field.Precision},{field.Scale})" : $"NUMBER({field.Precision})";
            }
            return "NUMBER";
        }

        /// <summary>
        /// Gets the timestamp type with the fractional seconds precision of the field.
        /// </summary>
        /// <param name="type">The name of the type.</param>
        /// <param name="field">The field of the column.</param>
        /// <returns>The name of the type.</returns>
        private static string ComposeTimestampType(string type,
            DbField field)
        {
            var precision = field.Scale.HasValue ? Math.Min(field.Scale.Value, (byte)9) : (byte)6;
            return type.Replace("timestamp", $"TIMESTAMP({precision})").Replace(" with time zone", " WITH TIME ZONE").Replace(" with local time zone", " WITH LOCAL TIME ZONE");
        }

        // Names

        /// <summary>
        /// Quotes the name with the quotes of the destination database.
        /// </summary>
        /// <param name="name">The name to be quoted.</param>
        /// <returns>The quoted name.</returns>
        private static string Quote(string name) =>
            OracleSchemaHelper.Quote(name);

        /// <summary>
        /// Quotes the name of a table, including its schema if it has one (i.e.: <c>schema.table</c>).
        /// </summary>
        /// <param name="name">The name of the table, with or without its schema.</param>
        /// <returns>The quoted name.</returns>
        private static string Name(string name) =>
            OracleSchemaHelper.QuoteName(name);

        /// <summary>
        /// Escapes a value to be used as a text literal in a SQL statement.
        /// </summary>
        /// <param name="value">The value to be escaped.</param>
        /// <returns>The escaped value (without the surrounding quotes).</returns>
        private static string Literal(string value) =>
            value.Replace("'", "''");

        /// <summary>
        /// Gets the expression of the owner in the data dictionary: the schema, or the current schema of the connection.
        /// </summary>
        /// <param name="schema">The name of the schema, or <c>null</c> to use the current schema.</param>
        /// <returns>The literal of the schema, or the expression of the current schema.</returns>
        private static string Owner(string schema) =>
            schema == null ? CurrentSchema : $"'{Literal(schema)}'";

        /// <summary>
        /// Gets the quoted name of the table of the schema.
        /// </summary>
        /// <param name="schema">The schema of the table.</param>
        /// <returns>The quoted name of the table, with its schema if it has one.</returns>
        private static string TableName(TableSchema schema) =>
            OracleSchemaHelper.FormatTableName(schema.Table.Schema, schema.Table.Name);

        /// <summary>
        /// Gets the quoted name of the table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>The quoted name of the table, with its schema if it has one.</returns>
        private static string TableName(TableInfo table) =>
            OracleSchemaHelper.FormatTableName(table.Schema, table.Name);

        /// <summary>
        /// Gets the quoted names of the columns as a comma-separated list.
        /// </summary>
        /// <param name="columns">The names of the columns.</param>
        /// <returns>The comma-separated quoted names of the columns.</returns>
        private static string Columns(IEnumerable<string> columns) =>
            string.Join(", ", columns.Select(Quote));

        /// <summary>
        /// The names that Oracle generates for the unnamed constraints are not given back, so the constraint is unnamed again.
        /// </summary>
        /// <param name="name">The name of the constraint.</param>
        /// <returns>The clause, followed by a space, or an empty text if the constraint has no name.</returns>
        private static string ConstraintName(string name) =>
            string.IsNullOrWhiteSpace(name) || name.StartsWith("SYS_C", StringComparison.Ordinal) ? string.Empty : $"CONSTRAINT {Quote(name)} ";

        /// <summary>
        /// Gets the text of the rule of a foreign key.
        /// </summary>
        /// <param name="rule">The rule of the foreign key.</param>
        /// <returns>The text of the rule, or <c>null</c> if the rule is the default of the database (or is not supported).</returns>
        private static string Rule(ForeignKeyRule rule)
        {
            switch (rule)
            {
                case ForeignKeyRule.Cascade: return "CASCADE";
                case ForeignKeyRule.SetNull: return "SET NULL";
                default: return null;
            }
        }

        // Columns

        /// <summary>
        /// Gets the definition of a column: its name, type, nullability and default value.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>The definition of the column.</returns>
        private string Column(ColumnInfo column)
        {
            var field = column.Field;
            var definition = new StringBuilder(Quote(field.Name)).Append(' ').Append(ComposeTypeName(column));
            var collation = SchemaCollation.Resolve(column, "ORACLE");
            if (collation != null)
            {
                definition.Append(" COLLATE ").Append(collation);
            }

            if (!string.IsNullOrWhiteSpace(column.ComputedExpression))
            {
                return definition.Append(" GENERATED ALWAYS AS (").Append(column.ComputedExpression).Append(") VIRTUAL").ToString();
            }
            if (field.IsIdentity)
            {
                definition.Append(" GENERATED BY DEFAULT AS IDENTITY (START WITH ").Append(column.IdentitySeed ?? 1).Append(" INCREMENT BY ").Append(column.IdentityIncrement ?? 1).Append(')');
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
