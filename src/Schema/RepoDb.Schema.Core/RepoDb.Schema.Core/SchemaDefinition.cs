#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Builds the versioned schema definition without serializing runtime schema objects or CLR types directly.
    /// </summary>
    internal static class SchemaDefinition
    {
        #region Public Methods

        /// <summary>
        /// Creates a schema definition for the selected tables and relationship behavior.
        /// </summary>
        /// <param name="root">The identity of the requested table.</param>
        /// <param name="tables">The table schemas included in the definition.</param>
        /// <param name="behavior">The relationship behavior used to select the tables.</param>
        /// <returns>The versioned schema definition.</returns>
        internal static JsonObject Create(TableInfo root,
            IList<TableSchema> tables,
            RelatedTableBehavior behavior)
        {
            return new JsonObject
            {
                ["format"] = "RepoDB.Schema",
                ["formatVersion"] = 2,
                ["source"] = new JsonObject { ["provider"] = root.Provider, ["database"] = root.Database },
                ["extraction"] = new JsonObject { ["rootTable"] = Table(root, root), ["relationshipBehavior"] = behavior.ToString() },
                ["tables"] = Array(tables.OrderBy(t => t.Table.Schema, StringComparer.Ordinal)
                    .ThenBy(t => t.Table.Name, StringComparer.Ordinal).Select(t => TableDefinition(t, behavior, root)))
            };
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates an array from definition nodes.
        /// </summary>
        /// <param name="items">The definition nodes.</param>
        /// <returns>The JSON array.</returns>
        private static JsonArray Array(IEnumerable<JsonNode> items) =>
            new JsonArray(items.ToArray());

        /// <summary>
        /// Creates an array of strings without reflection-based serialization.
        /// </summary>
        /// <param name="items">The string values.</param>
        /// <returns>The JSON array.</returns>
        private static JsonArray Strings(IEnumerable<string> items) =>
            Array(items.Select(item => (JsonNode)JsonValue.Create(item)));

        /// <summary>
        /// Creates a definition of a table identity.
        /// </summary>
        /// <param name="table">The table identity, or null.</param>
        /// <param name="source">The common source identity metadata.</param>
        /// <returns>The table identity definition, or null.</returns>
        private static JsonObject Table(TableInfo table, TableInfo source)
        {
            if (table == null) return null;
            var result = new JsonObject { ["schema"] = table.Schema, ["name"] = table.Name };
            // Retain overrides for identities outside the common source context.
            if (table.Database != source.Database) result["database"] = table.Database;
            if (table.Provider != source.Provider) result["provider"] = table.Provider;
            return result;
        }

        /// <summary>
        /// Creates a table definition containing its columns, keys, indexes, and constraints.
        /// </summary>
        /// <param name="table">The table schema to describe.</param>
        /// <param name="behavior">The relationship behavior controlling foreign-key inclusion.</param>
        /// <param name="source">The common source identity metadata.</param>
        /// <returns>The complete table definition.</returns>
        private static JsonObject TableDefinition(TableSchema table,
            RelatedTableBehavior behavior,
            TableInfo source) => new JsonObject
        {
            ["table"] = Table(table.Table, source),
            ["columns"] = Array(table.Columns.OrderBy(c => c.Ordinal).Select(Column)),
            ["primaryKey"] = table.PrimaryKey == null ? null : new JsonObject
            {
                ["name"] = table.PrimaryKey.Name, ["columns"] = Strings(table.PrimaryKey.Columns),
                ["clustered"] = table.PrimaryKey.IsClustered
            },
            ["uniqueConstraints"] = Array(table.UniqueConstraints.OrderBy(c => c.Name, StringComparer.Ordinal).Select(c => new JsonObject
            { ["name"] = c.Name, ["columns"] = Strings(c.Columns) })),
            ["indexes"] = Array(table.Indexes.OrderBy(i => i.Name, StringComparer.Ordinal).Select(i => new JsonObject
            {
                ["name"] = i.Name, ["unique"] = i.IsUnique, ["clustered"] = i.IsClustered,
                ["columns"] = Array(i.Columns.Select(c => new JsonObject
                { ["name"] = c, ["direction"] = i.DescendingColumns.Contains(c) ? "descending" : "ascending" })),
                ["includedColumns"] = Strings(i.IncludedColumns), ["filter"] = Expression(i.Filter)
            })),
            ["checkConstraints"] = Array(table.CheckConstraints.OrderBy(c => c.Name, StringComparer.Ordinal).Select(c => new JsonObject
            { ["name"] = c.Name, ["expression"] = Expression(c.Expression) })),
            ["foreignKeys"] = behavior == RelatedTableBehavior.TableOnly ? new JsonArray() : Array(
                table.ForeignKeys.OrderBy(f => f.Name, StringComparer.Ordinal).Select(f => new JsonObject
                {
                    ["name"] = f.Name, ["columns"] = Strings(f.Columns),
                    ["references"] = new JsonObject
                    {
                        ["table"] = Table(f.ReferencedTable, source), ["columns"] = Strings(f.ReferencedColumns)
                    },
                    ["onDelete"] = f.DeleteRule.ToString(), ["onUpdate"] = f.UpdateRule.ToString()
                }))
        };

        /// <summary>
        /// Creates a definition of a native SQL expression; its dialect comes from the owning table or field.
        /// </summary>
        /// <param name="sql">The SQL expression, or null.</param>
        /// <returns>The expression definition, or null.</returns>
        private static JsonObject Expression(string sql) => sql == null ? null : new JsonObject
        { ["sql"] = sql };

        /// <summary>
        /// Creates a column definition while preserving its type and provider metadata.
        /// </summary>
        /// <param name="column">The column metadata to describe.</param>
        /// <returns>The column definition.</returns>
        private static JsonObject Column(ColumnInfo column)
        {
            var field = column.Field ?? throw new InvalidOperationException("A schema column has no field metadata.");
            var native = field.DatabaseType?.ToLowerInvariant();
            var baseType = native?.Split('(', ' ')[0];
            var text = field.Type == typeof(string) || field.Type == typeof(char);
            bool? unicode = column.IsUnicode;
            var charset = column.CharacterSet;
            var unit = column.LengthUnit;
            if (text && string.Equals(field.Provider, "SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(baseType, "nvarchar", StringComparison.Ordinal) || string.Equals(baseType, "nchar", StringComparison.Ordinal) || string.Equals(baseType, "ntext", StringComparison.Ordinal))
                {
                    unicode = unicode ?? true;
                    charset = charset ?? "UTF-16";
                    unit = unit ?? "utf16CodeUnits";
                }
                else if (string.Equals(baseType, "varchar", StringComparison.Ordinal) || string.Equals(baseType, "char", StringComparison.Ordinal) || string.Equals(baseType, "text", StringComparison.Ordinal))
                {
                    // A UTF-8 collation changes varchar's character repertoire.
                    if (column.Collation != null) unicode = unicode ?? column.Collation.EndsWith("_UTF8", StringComparison.OrdinalIgnoreCase);
                    if (unicode == true) charset = charset ?? "UTF-8";
                    unit = unit ?? "bytes";
                }
            }
            return new JsonObject
            {
                ["name"] = field.Name, ["ordinal"] = column.Ordinal,
                ["type"] = new JsonObject
                {
                    ["logical"] = LogicalType(field.Type), ["native"] = column.NativeType ?? field.DatabaseType, ["provider"] = field.Provider,
                    ["length"] = field.Size, ["lengthUnit"] = unit,
                    ["precision"] = column.NumericPrecision ?? field.Precision, ["scale"] = column.NumericScale ?? field.Scale, ["temporalPrecision"] = column.TemporalPrecision,
                    ["unicode"] = unicode, ["characterSet"] = charset
                },
                ["nullable"] = field.IsNullable,
                ["identity"] = !field.IsIdentity ? null : new JsonObject { ["seed"] = column.IdentitySeed, ["increment"] = column.IdentityIncrement },
                ["default"] = !field.HasDefaultValue && column.DefaultExpression == null ? null : new JsonObject
                { ["kind"] = column.DefaultValue.ToString(), ["expression"] = Expression(column.DefaultExpression) },
                ["computed"] = Expression(column.ComputedExpression),
                ["collation"] = column.Collation, ["comment"] = column.Comment
            };
        }

        /// <summary>
        /// Resolves a client type to its portable definition type name.
        /// </summary>
        /// <param name="type">The client type reported by the schema reader.</param>
        /// <returns>The portable type name, or unknown if no mapping exists.</returns>
        private static string LogicalType(Type type)
        {
            if (type == null) return "unknown";
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type == typeof(string) || type == typeof(char)) return "string";
            if (type == typeof(byte[])) return "binary";
            if (type == typeof(Guid)) return "uuid";
            if (type == typeof(DateTimeOffset)) return "datetimeOffset";
            if (type == typeof(TimeSpan)) return "time";
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean: return "boolean";
                case TypeCode.Byte: return "uint8";
                case TypeCode.SByte: return "int8";
                case TypeCode.Int16: return "int16";
                case TypeCode.UInt16: return "uint16";
                case TypeCode.Int32: return "int32";
                case TypeCode.UInt32: return "uint32";
                case TypeCode.Int64: return "int64";
                case TypeCode.UInt64: return "uint64";
                case TypeCode.Decimal: return "decimal";
                case TypeCode.Single: return "float32";
                case TypeCode.Double: return "float64";
                case TypeCode.DateTime: return "datetime";
                default: return "unknown";
            }
        }

        #endregion
    }
}
