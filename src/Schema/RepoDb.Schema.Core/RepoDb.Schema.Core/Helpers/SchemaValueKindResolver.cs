#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Resolves the kind of the typed values of a column, from its .NET type and its native type.
    /// </summary>
    internal static class SchemaValueKindResolver
    {
        #region Private Variables

        private static readonly Dictionary<string, SchemaValueKind> Kinds = new Dictionary<string, SchemaValueKind>(StringComparer.Ordinal)
        {
            ["System.Boolean"] = SchemaValueKind.Boolean,
            ["System.Byte"] = SchemaValueKind.Number,
            ["System.Byte[]"] = SchemaValueKind.Binary,
            ["System.Char"] = SchemaValueKind.Text,
            ["System.DateOnly"] = SchemaValueKind.Date,
            ["System.DateTime"] = SchemaValueKind.DateTime,
            ["System.DateTimeOffset"] = SchemaValueKind.DateTimeOffset,
            ["System.Decimal"] = SchemaValueKind.Number,
            ["System.Double"] = SchemaValueKind.Number,
            ["System.Guid"] = SchemaValueKind.Guid,
            ["System.Int16"] = SchemaValueKind.Number,
            ["System.Int32"] = SchemaValueKind.Number,
            ["System.Int64"] = SchemaValueKind.Number,
            ["System.SByte"] = SchemaValueKind.Number,
            ["System.Single"] = SchemaValueKind.Number,
            ["System.String"] = SchemaValueKind.Text,
            ["System.TimeOnly"] = SchemaValueKind.Time,
            ["System.TimeSpan"] = SchemaValueKind.Time,
            ["System.UInt16"] = SchemaValueKind.Number,
            ["System.UInt32"] = SchemaValueKind.Number,
            ["System.UInt64"] = SchemaValueKind.Number
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Resolves the kind of the typed values of the column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="dialect">The dialect of the database of the column, which tells its native types that hold a date only (the standard one if it is <c>null</c>).</param>
        /// <returns>The kind, or <c>null</c> if the type of the column has no equivalent.</returns>
        public static SchemaValueKind? Resolve(ColumnInfo column,
            ISchemaDialect dialect)
        {
            var field = column?.Field;
            if (field?.Type == null)
            {
                return null;
            }

            var type = Nullable.GetUnderlyingType(field.Type) ?? field.Type;
            if (!Kinds.TryGetValue(type.FullName ?? string.Empty,
                out var kind))
            {
                return null;
            }

            return kind == SchemaValueKind.DateTime && (dialect ?? SchemaDialect.Standard).IsDateOnly(column.NativeType ?? field.DatabaseType) ? SchemaValueKind.Date : kind;
        }

        #endregion
    }
}
