#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Reconstructs schema models from the versioned portable definition shared by the file formats.
    /// </summary>
    internal static class SchemaDefinitionReader
    {
        #region Public Methods

        /// <summary>
        /// Recognizes the document and reconstructs its tables without consulting a database. The reconstructed definition is not validated: the validation is a separate step.
        /// </summary>
        /// <param name="document">The definition document that is read from the file.</param>
        /// <returns>The definition that is reconstructed from the document.</returns>
        internal static SchemaDefinitionInfo Read(JsonObject document)
        {
            try
            {
                if (document == null)
                    throw new InvalidDataException("The file is not a RepoDB schema definition.");
                var format = Text(document,
                    "format");
                var version = Value<int>(document,
                    "formatVersion");
                if (version != 1 && version != 2)
                    throw new InvalidDataException("The schema definition version is not supported.");
                var source = Object(document,
                    "source");
                var extraction = Object(document,
                    "extraction");
                var result = new SchemaDefinitionInfo
                {
                    Format = format,
                    FormatVersion = version,
                    Provider = Text(source,
                        "provider"),
                    Database = Text(source,
                        "database"),
                    RootTable = Table(Object(extraction,
                        "rootTable"),
                        source),
                    RelationshipBehavior = Enumeration<RelatedTableBehavior>(extraction,
                        "relationshipBehavior"),
                    Tables = Array(document,
                        "tables").Select(t => ReadTable(ObjectValue(t),
                            source)).ToList(),
                    ExternalDependencies = version == 1 ? Array(document,
                        "externalDependencies").Select(t => Table(ObjectValue(t),
                            source)).ToList() : null,
                    Definition = document
                };
                if (version == 2)
                {
                    var included = new HashSet<TableInfo>(result.Tables.Select(t => t.Table));
                    result.ExternalDependencies = result.Tables.SelectMany(t => t.ForeignKeys).Select(f => f.ReferencedTable).Where(t => t != null && !included.Contains(t)).Distinct().OrderBy(t => t.Schema,
                        StringComparer.Ordinal).ThenBy(t => t.Name,
                            StringComparer.Ordinal).ToList();
                }

                return result;
            }
            catch (Exception exception) when (exception is InvalidOperationException || exception is FormatException || exception is ArgumentException || exception is OverflowException || exception is JsonException)
            {
                throw new InvalidDataException("The schema definition contains invalid or missing metadata.",
                    exception);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reconstructs one table and its ordered keys and constraints.
        /// </summary>
        /// <param name="node">The table node of the document.</param>
        /// <param name="source">The document source metadata.</param>
        /// <returns>The reconstructed table.</returns>
        private static TableSchema ReadTable(JsonObject node,
            JsonObject source)
        {
            var table = new TableSchema(Table(Object(node,
                "table"),
                source));
            if (!node.ContainsKey("primaryKey"))
                throw new InvalidDataException("Missing primaryKey metadata.");
            if (node["primaryKey"] is JsonObject primary)
                table.PrimaryKey = new PrimaryKeyInfo(Name(primary))
                {
                    Columns = Strings(primary,
                        "columns"),
                    IsClustered = Value<bool>(primary,
                        "clustered")
                };
            else if (node["primaryKey"] != null)
                throw new InvalidDataException("Invalid primaryKey metadata.");
            table.Columns = Array(node,
                "columns").Select(c => Column(ObjectValue(c),
                    table.PrimaryKey)).ToList();
            table.UniqueConstraints = Array(node,
                "uniqueConstraints").Select(c => new UniqueConstraintInfo(Name(c)) { Columns = Strings(c,
                    "columns") }).ToList();
            table.CheckConstraints = Array(node,
                "checkConstraints").Select(c => new CheckConstraintInfo(Name(c)) { Expression = Expression(c,
                    "expression") }).ToList();
            table.Indexes = Array(node,
                "indexes").Select(Index).ToList();
            table.ForeignKeys = Array(node,
                "foreignKeys").Select(f =>
            {
                var reference = Object(f,
                    "references");
                return new ForeignKeyInfo(Name(f))
                {
                    Columns = Strings(f,
                        "columns"),
                    ReferencedTable = reference["table"] == null ? null : Table(Object(reference,
                        "table"),
                        source),
                    ReferencedColumns = Strings(reference,
                        "columns"),
                    DeleteRule = Enumeration<ForeignKeyRule>(f,
                        "onDelete"),
                    UpdateRule = Enumeration<ForeignKeyRule>(f,
                        "onUpdate")
                };
            }).ToList();
            return table;
        }

        /// <summary>
        /// Reconstructs index ordering, included columns, flags, and its native SQL filter.
        /// </summary>
        /// <param name="node">The index node of the table.</param>
        /// <returns>The reconstructed index.</returns>
        private static IndexInfo Index(JsonNode node)
        {
            var columns = Array(node,
                "columns");
            foreach (var column in columns)
                if (!string.Equals(Text(column,
                    "direction"),
                    "ascending",
                    StringComparison.Ordinal) && !string.Equals(Text(column,
                        "direction"),
                        "descending",
                        StringComparison.Ordinal))
                    throw new InvalidDataException("Invalid index column direction.");
            return new IndexInfo(Name(node))
            {
                Columns = columns.Select(Name).ToList(),
                DescendingColumns = columns.Where(c => string.Equals(Text(c,
                    "direction"),
                    "descending",
                    StringComparison.Ordinal)).Select(Name).ToList(),
                IncludedColumns = Strings(node,
                    "includedColumns"),
                IsUnique = Value<bool>(node,
                    "unique"),
                IsClustered = Value<bool>(node,
                    "clustered"),
                Filter = Expression(node,
                    "filter")
            };
        }

        /// <summary>
        /// Restores portable column metadata; numeric metadata outside the byte range stays on ColumnInfo.
        /// </summary>
        /// <param name="node">The column node of the table.</param>
        /// <param name="primary">The primary key of the table, or <c>null</c> if the table has none.</param>
        /// <returns>The reconstructed column.</returns>
        private static ColumnInfo Column(JsonObject node,
            PrimaryKeyInfo primary)
        {
            foreach (var property in new[]
            {
                "identity",
                "default",
                "computed",
                "collation",
                "comment"
            }

            )
                if (!node.ContainsKey(property))
                    throw new InvalidDataException("Missing column property: " + property);
            var type = Object(node,
                "type");
            var identity = node["identity"]?.AsObject();
            var defaultValue = node["default"]?.AsObject();
            var precision = Optional<int>(type,
                "precision");
            var scale = Optional<int>(type,
                "scale");
            return new ColumnInfo
            {
                Field = ReadField(node,
                    type,
                    primary,
                    identity,
                    defaultValue,
                    precision,
                    scale),
                Ordinal = Value<int>(node,
                    "ordinal"),
                NativeType = Text(type,
                    "native"),
                NumericPrecision = precision,
                NumericScale = scale,
                TemporalPrecision = Optional<int>(type,
                    "temporalPrecision"),
                IsUnicode = Optional<bool>(type,
                    "unicode"),
                CharacterSet = Text(type,
                    "characterSet"),
                LengthUnit = Text(type,
                    "lengthUnit"),
                IdentitySeed = identity == null ? null : Optional<long>(identity,
                    "seed"),
                IdentityIncrement = identity == null ? null : Optional<long>(identity,
                    "increment"),
                DefaultValue = defaultValue == null ? ColumnDefaultValue.Unclassified : Enumeration<ColumnDefaultValue>(defaultValue,
                    "kind"),
                DefaultExpression = defaultValue == null ? null : Expression(defaultValue,
                    "expression"),
                ComputedExpression = Expression(node,
                    "computed"),
                Collation = Text(node,
                    "collation"),
                Comment = Text(node,
                    "comment")
            };
        }

        /// <summary>
        /// Reconstructs a table identity, including its source metadata.
        /// </summary>
        /// <param name="node">The node of the table identity.</param>
        /// <param name="source">The inherited source metadata, if available.</param>
        /// <returns>The identity of the table.</returns>
        private static TableInfo Table(JsonObject node,
            JsonObject source = null) =>
            new TableInfo(Name(node),
                Text(node,
                    "schema"),
                node.ContainsKey("database") ? Text(node,
                    "database") : source == null ? null : Text(source,
                    "database"),
                node.ContainsKey("provider") ? Text(node,
                    "provider") : source == null ? null : Text(source,
                    "provider"));

        /// <summary>
        /// Reads a required object.
        /// </summary>
        /// <param name="node">The parent node.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The object that the property holds.</returns>
        private static JsonObject Object(JsonNode node,
            string name) =>
            (node as JsonObject)?[name] as JsonObject ?? throw new InvalidDataException("Missing object: " + name);

        /// <summary>
        /// Reads an object in a collection, rejecting null and scalar entries.
        /// </summary>
        /// <param name="node">The node of the collection entry.</param>
        /// <returns>The entry as an object.</returns>
        private static JsonObject ObjectValue(JsonNode node) =>
            node as JsonObject ?? throw new InvalidDataException("Expected a definition object.");

        /// <summary>
        /// Reads a required array.
        /// </summary>
        /// <param name="node">The parent node.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The array that the property holds.</returns>
        private static JsonArray Array(JsonNode node,
            string name) =>
            (node as JsonObject)?[name] as JsonArray ?? throw new InvalidDataException("Missing array: " + name);

        /// <summary>
        /// Reads a required property that can contain a null string.
        /// </summary>
        /// <param name="node">The parent node.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The text that the property holds, or <c>null</c>.</returns>
        private static string Text(JsonNode node,
            string name)
        {
            if (!(node is JsonObject obj) || !obj.ContainsKey(name))
                throw new InvalidDataException("Missing property: " + name);
            return node[name]?.GetValue<string>();
        }

        /// <summary>
        /// Reads a nonempty object name.
        /// </summary>
        /// <param name="node">The node that has the name.</param>
        /// <returns>The name.</returns>
        private static string Name(JsonNode node)
        {
            var name = Text(node,
                "name");
            return !string.IsNullOrWhiteSpace(name) ? name : throw new InvalidDataException("A definition name is empty.");
        }

        /// <summary>
        /// Reads an ordered string collection.
        /// </summary>
        /// <param name="node">The parent node.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The strings that the property holds, in order.</returns>
        private static IList<string> Strings(JsonNode node,
            string name) =>
            Array(node,
                name).Select(n => n?.GetValue<string>() ?? throw new InvalidDataException("Null column name.")).ToList();

        /// <summary>
        /// Reads a required scalar value.
        /// </summary>
        /// <param name="node">The parent node.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The value that the property holds.</returns>
        private static T Value<T>(JsonNode node,
            string name) =>
            node[name] == null ? throw new InvalidDataException("Missing value: " + name) : node[name].GetValue<T>();

        /// <summary>
        /// Reads a nullable scalar value.
        /// </summary>
        /// <param name="node">The parent node.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The value that the property holds, or <c>null</c> if it is missing.</returns>
        private static T? Optional<T>(JsonNode node,
            string name)
            where T : struct =>
            node[name] == null ? (T? )null : node[name].GetValue<T>();

        /// <summary>
        /// Reads a named enum value, rejecting numeric and unknown values.
        /// </summary>
        /// <param name="node">The parent node.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The enumeration value that the property names.</returns>
        private static T Enumeration<T>(JsonNode node,
            string name)
            where T : struct
        {
            var text = Text(node,
                name);
            if (!Enum.TryParse<T>(text,
                out var value) || !Enum.IsDefined(typeof(T),
                    value) || !string.Equals(value.ToString(),
                    text,
                    StringComparison.Ordinal))
                throw new InvalidDataException("Invalid enum value: " + name);
            return value;
        }

        /// <summary>
        /// Reads the native SQL from an optional expression, retaining dialect metadata in the document.
        /// </summary>
        /// <param name="node">The parent node.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The native SQL of the expression, or <c>null</c> if there is no expression.</returns>
        private static string Expression(JsonNode node,
            string name) =>
            node[name] == null ? null : Text(Object(node,
                name),
                "sql");

        /// <summary>
        /// Converts metadata to DbField's restricted range without losing the original ColumnInfo value.
        /// </summary>
        /// <param name="value">The numeric metadata.</param>
        /// <returns>The value as a byte, or <c>null</c> if it is missing or outside the byte range.</returns>
        private static byte? Byte(int? value) =>
            value >= 0 && value <= byte.MaxValue ? (byte? )value.Value : null;

        /// <summary>
        /// Resolves only known portable logical types; it never loads CLR types named by a file.
        /// </summary>
        /// <param name="name">The portable logical type name.</param>
        /// <returns>The CLR type of the logical type, or <c>null</c> for an unknown type.</returns>
        private static Type LogicalType(string name)
        {
            switch (name)
            {
                case "string":
                    return typeof(string);
                case "binary":
                    return typeof(byte[]);
                case "uuid":
                    return typeof(Guid);
                case "datetimeOffset":
                    return typeof(DateTimeOffset);
                case "time":
                    return typeof(TimeSpan);
                case "boolean":
                    return typeof(bool);
                case "uint8":
                    return typeof(byte);
                case "int8":
                    return typeof(sbyte);
                case "int16":
                    return typeof(short);
                case "uint16":
                    return typeof(ushort);
                case "int32":
                    return typeof(int);
                case "uint32":
                    return typeof(uint);
                case "int64":
                    return typeof(long);
                case "uint64":
                    return typeof(ulong);
                case "decimal":
                    return typeof(decimal);
                case "float32":
                    return typeof(float);
                case "float64":
                    return typeof(double);
                case "datetime":
                    return typeof(DateTime);
                case "unknown":
                    return null;
                default:
                    throw new InvalidDataException("Unknown logical type: " + name);
            }
        }

        /// <summary>
        /// Reconstructs the runtime field from portable metadata and primary-key membership.
        /// </summary>
        /// <param name="node">The column node of the table.</param>
        /// <param name="type">The portable type metadata.</param>
        /// <param name="primary">The primary key of the table, or <c>null</c> if the table has none.</param>
        /// <param name="identity">The identity metadata, or null.</param>
        /// <param name="defaultValue">The default value metadata, or null.</param>
        /// <param name="precision">The portable numeric precision.</param>
        /// <param name="scale">The portable numeric scale.</param>
        /// <returns>The reconstructed runtime field.</returns>
        private static DbField ReadField(JsonObject node,
            JsonObject type,
            PrimaryKeyInfo primary,
            JsonObject identity,
            JsonObject defaultValue,
            int? precision,
            int? scale)
        {
            var name = Name(node);
            return new DbField(name,
                primary?.Columns.Contains(name) == true,
                identity != null,
                Value<bool>(node,
                    "nullable"),
                LogicalType(Text(type,
                    "logical")),
                Optional<int>(type,
                    "length"),
                Byte(precision),
                Byte(scale),
                Text(type,
                    "native"),
                defaultValue != null,
                Text(type,
                    "provider"));
        }

        #endregion
    }
}
