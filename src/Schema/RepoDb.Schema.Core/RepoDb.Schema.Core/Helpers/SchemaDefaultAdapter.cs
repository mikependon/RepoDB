#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Linq;
using RepoDb.Interfaces;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Converts the default values of the columns of a table that comes from a database of another provider: the literals are parsed into typed values
    /// and written in the format of the destination database, so a boolean, a date, a time or an offset is written the way the destination accepts it.
    /// A default value that can not be converted is left out and reported as a warning, so the table is still created.
    /// </summary>
    internal static class SchemaDefaultAdapter
    {
        #region Private Variables

        private static readonly ColumnDefaultValue[] LiteralKinds =
        {
            ColumnDefaultValue.BooleanFalse,
            ColumnDefaultValue.BooleanTrue,
            ColumnDefaultValue.Literal
        };
        private static readonly ColumnDefaultValue[] NativeKinds =
        {
            ColumnDefaultValue.CurrentDatabase,
            ColumnDefaultValue.CurrentDate,
            ColumnDefaultValue.CurrentTime,
            ColumnDefaultValue.CurrentTimestamp,
            ColumnDefaultValue.CurrentTimestampUtc,
            ColumnDefaultValue.CurrentTimestampWithOffset,
            ColumnDefaultValue.CurrentUser,
            ColumnDefaultValue.Expression,
            ColumnDefaultValue.NewGuid,
            ColumnDefaultValue.NewSequentialGuid,
            ColumnDefaultValue.RandomNumber,
            ColumnDefaultValue.SequenceNextValue
        };

        #endregion

        #region Private Methods

        /// <summary>
        /// Adapts the default value of a column.
        /// </summary>
        /// <param name="table">The state of the table, which receives the warnings.</param>
        /// <param name="column">The column of the source table.</param>
        /// <param name="composer">The composer of the destination database.</param>
        /// <param name="formatter">The formatter of the destination database.</param>
        /// <param name="target">The dialect of the destination database.</param>
        /// <returns>The column for the destination database.</returns>
        private static ColumnInfo AdaptColumn(PlanTable table,
            ColumnInfo column,
            ISchemaComposer composer,
            ISchemaValueFormatter formatter,
            ISchemaDialect target)
        {
            var source = SchemaDialectMapper.Get(column.Field?.Provider ?? table.Schema.Table.Provider);
            if (!IsConvertible(column,
                formatter,
                source,
                target))
            {
                return column;
            }

            var expression = Compose(column,
                formatter,
                source,
                composer.ComposeTypeName(column),
                out var warning,
                out var reason);
            if (expression == null)
            {
                table.Warnings.Add($"Skipped the default value of the column '{column.Field?.Name}': {reason}.");
                return Clone(column,
                    null,
                    ColumnDefaultValue.None);
            }

            if (warning != null)
            {
                table.Warnings.Add($"Converted the default value of the column '{column.Field?.Name}': {warning}.");
            }

            return Clone(column,
                expression,
                ColumnDefaultValue.Unclassified);
        }

        /// <summary>
        /// Copies a column with another default value.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="expression">The default expression.</param>
        /// <param name="kind">The kind of the default value.</param>
        /// <returns>The new column.</returns>
        private static ColumnInfo Clone(ColumnInfo column,
            string expression,
            ColumnDefaultValue kind) =>
            new ColumnInfo
            {
                CharacterSet = column.CharacterSet,
                Collation = column.Collation,
                Comment = column.Comment,
                ComputedExpression = column.ComputedExpression,
                DefaultExpression = expression,
                DefaultValue = kind,
                Field = column.Field,
                IdentityIncrement = column.IdentityIncrement,
                IdentitySeed = column.IdentitySeed,
                IsUnicode = column.IsUnicode,
                LengthUnit = column.LengthUnit,
                NativeType = column.NativeType,
                NumericPrecision = column.NumericPrecision,
                NumericScale = column.NumericScale,
                Ordinal = column.Ordinal,
                TemporalPrecision = column.TemporalPrecision
            };

        /// <summary>
        /// Writes the default value of the column in the format of the destination database.
        /// </summary>
        /// <param name="column">The column of the source table.</param>
        /// <param name="formatter">The formatter of the destination database.</param>
        /// <param name="source">The dialect of the source database.</param>
        /// <param name="destinationType">The type of the column in the destination database.</param>
        /// <param name="warning">The reason why the value was changed, or <c>null</c>.</param>
        /// <param name="reason">The reason why the default value can not be written, when this method returns <c>null</c>.</param>
        /// <returns>The text that follows <c>DEFAULT</c>, or <c>null</c> if it can not be written.</returns>
        private static string Compose(ColumnInfo column,
            ISchemaValueFormatter formatter,
            ISchemaDialect source,
            string destinationType,
            out string warning,
            out string reason)
        {
            warning = null;
            reason = null;
            if (column.DefaultValue == ColumnDefaultValue.Expression)
            {
                reason = $"its expression '{column.DefaultExpression}' is written in the language of {column.Field?.Provider ?? "another database"}, which the destination does not understand";
                return null;
            }

            if (NativeKinds.Contains(column.DefaultValue))
            {
                var text = formatter.FormatKind(column.DefaultValue,
                    destinationType,
                    out warning);
                reason = text == null ? $"it is {column.DefaultValue}, which the destination has no equivalent for" : null;
                return text;
            }

            return ComposeLiteral(column,
                formatter,
                source,
                destinationType,
                out warning,
                out reason);
        }

        /// <summary>
        /// Writes a literal default value in the format of the destination database.
        /// </summary>
        /// <param name="column">The column of the source table.</param>
        /// <param name="formatter">The formatter of the destination database.</param>
        /// <param name="source">The dialect of the source database.</param>
        /// <param name="destinationType">The type of the column in the destination database.</param>
        /// <param name="warning">The reason why the value was changed, or <c>null</c>.</param>
        /// <param name="reason">The reason why the default value can not be written, when this method returns <c>null</c>.</param>
        /// <returns>The text that follows <c>DEFAULT</c>, or <c>null</c> if it can not be written.</returns>
        private static string ComposeLiteral(ColumnInfo column,
            ISchemaValueFormatter formatter,
            ISchemaDialect source,
            string destinationType,
            out string warning,
            out string reason)
        {
            warning = null;
            var value = GetValue(column,
                source);
            if (value == null)
            {
                reason = $"its value '{column.DefaultExpression}' is not a literal that is understood for the type of the column";
                return null;
            }

            var text = formatter.Format(value,
                destinationType,
                out warning);
            reason = text == null ? $"the destination can not hold a {value.Kind} value as a default value" : null;
            return text;
        }

        /// <summary>
        /// Gets the typed value of a literal default value.
        /// </summary>
        /// <param name="column">The column of the source table.</param>
        /// <param name="source">The dialect of the source database.</param>
        /// <returns>The value, or <c>null</c> if it is not understood.</returns>
        private static SchemaValue GetValue(ColumnInfo column,
            ISchemaDialect source)
        {
            if (column.DefaultValue == ColumnDefaultValue.BooleanTrue || column.DefaultValue == ColumnDefaultValue.BooleanFalse)
            {
                return SchemaValue.FromBoolean(column.DefaultValue == ColumnDefaultValue.BooleanTrue);
            }

            var kind = SchemaValueKindResolver.Resolve(column,
                source);
            return kind == null ? null : SchemaValueParser.Parse(kind.Value,
                column.DefaultExpression,
                source);
        }

        /// <summary>
        /// Checks whether the default value of the column is converted: it comes from a provider of another dialect, and it is a literal
        /// (or a kind or an expression, if the formatter writes them).
        /// </summary>
        /// <param name="column">The column of the source table.</param>
        /// <param name="formatter">The formatter of the destination database.</param>
        /// <param name="source">The dialect of the provider of the column (<c>null</c> if it is not registered).</param>
        /// <param name="target">The dialect of the destination database.</param>
        /// <returns><c>true</c> if the default value is converted.</returns>
        private static bool IsConvertible(ColumnInfo column,
            ISchemaValueFormatter formatter,
            ISchemaDialect source,
            ISchemaDialect target) =>
            source != null && source.Family != target.Family && (LiteralKinds.Contains(column.DefaultValue) || (formatter.HandlesKinds && NativeKinds.Contains(column.DefaultValue)));

        /// <summary>
        /// Creates the schema of the table with the columns.
        /// </summary>
        /// <param name="schema">The schema of the table.</param>
        /// <param name="columns">The columns.</param>
        /// <returns>The new schema of the table.</returns>
        private static TableSchema WithColumns(TableSchema schema,
            IList<ColumnInfo> columns) =>
            new TableSchema(schema.Table)
            {
                CheckConstraints = schema.CheckConstraints,
                Columns = columns,
                ForeignKeys = schema.ForeignKeys,
                Indexes = schema.Indexes,
                PrimaryKey = schema.PrimaryKey,
                UniqueConstraints = schema.UniqueConstraints
            };

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts the default values of the table to the destination database, if the composer of the destination gives a formatter
        /// (see <see cref="ISchemaValueSupport"/>) and the table comes from a dialect of another family (see <see cref="SchemaDialectMapper"/>). The schema that was read is not changed: the table gets a new schema.
        /// </summary>
        /// <param name="table">The state of the table.</param>
        /// <param name="composer">The composer of the destination database.</param>
        /// <param name="destination">The setting of the destination database, which gives its provider.</param>
        public static void Adapt(PlanTable table,
            ISchemaComposer composer,
            IDbSetting destination)
        {
            var target = SchemaDialectMapper.Get(destination?.Provider);
            if (!(composer is ISchemaValueSupport support) || target == null || table.Schema.Columns.Count == 0)
            {
                return;
            }

            var columns = table.Schema.Columns.Select(column => AdaptColumn(table,
                column,
                composer,
                support.ValueFormatter,
                target)).ToList();
            if (columns.Where((column,
                index) => !ReferenceEquals(column,
                table.Schema.Columns[index])).Any())
            {
                table.Schema = WithColumns(table.Schema,
                    columns);
            }
        }

        #endregion
    }
}
