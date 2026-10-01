#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using RepoDb.DbSettings;
using RepoDb.Enumerations.SqlServer;
using RepoDb.Exceptions;
using RepoDb.Extensions;

namespace RepoDb
{
    public static partial class SqlConnectionExtension
    {
        #region ColumnMappings

        /// <summary>
        /// The name of the internal column that the bulk operations add to a <see cref="System.Data.DataTable"/> to preserve
        /// the order of its rows. It is never a column of the source data.
        /// </summary>
        private const string OrderColumnName = "__RepoDb_OrderColumn";

        /// <summary>
        /// Resolves the destination fields to be mapped from the source columns, based on the
        /// <see cref="SqlServerBulkOperationsDbSetting.BulkColumnMappingsBehavior"/> in used. This is only called when no explicit
        /// mappings were passed to the operation, as explicit mappings always take precedence over the behavior.
        /// </summary>
        /// <param name="connection">The connection object, whose <see cref="SqlServerBulkOperationsDbSetting"/> (if registered) defines the behavior.</param>
        /// <param name="tableName">The name of the destination table.</param>
        /// <param name="fields">The fields of the destination table, in the order of the table columns.</param>
        /// <param name="sourceColumnNames">The names of the source columns, in order.</param>
        /// <returns>The destination fields to be mapped, in the order of the table columns.</returns>
        private static IEnumerable<Field> GetFieldsForColumnMappings(IDbConnection connection,
            string tableName,
            IEnumerable<Field> fields,
            IEnumerable<string> sourceColumnNames) =>
            GetFieldsForColumnMappings(tableName,
                fields,
                sourceColumnNames,
                (connection.GetDbSetting() as SqlServerBulkOperationsDbSetting)?.BulkColumnMappingsBehavior ?? default);

        /// <summary>
        /// Resolves the destination fields to be mapped from the source columns, based on the given column mappings behavior.
        /// </summary>
        /// <param name="tableName">The name of the destination table.</param>
        /// <param name="fields">The fields of the destination table, in the order of the table columns.</param>
        /// <param name="sourceColumnNames">The names of the source columns, in order.</param>
        /// <param name="behavior">The column mappings behavior to be applied.</param>
        /// <returns>The destination fields to be mapped, in the order of the table columns.</returns>
        internal static IEnumerable<Field> GetFieldsForColumnMappings(string tableName,
            IEnumerable<Field> fields,
            IEnumerable<string> sourceColumnNames,
            SqlServerBulkColumnMappingsBehavior behavior)
        {
            var sourceNames = sourceColumnNames?
                .Where(name => !string.Equals(name, OrderColumnName, StringComparison.OrdinalIgnoreCase))
                .AsList() ?? [];

            return behavior switch
            {
                SqlServerBulkColumnMappingsBehavior.Strict => GetFieldsForStrictColumnMappings(tableName, fields?.AsList() ?? [], sourceNames),
                SqlServerBulkColumnMappingsBehavior.StrictBypass => GetFieldsForStrictBypassColumnMappings(tableName, fields?.AsList() ?? [], sourceNames),
                _ => GetFieldsForAutomaticColumnMappings(fields, sourceNames)
            };
        }

        /// <summary>
        /// Automatic: maps the columns that exist on both sides, in any order (the behavior of the earlier versions).
        /// </summary>
        private static IEnumerable<Field> GetFieldsForAutomaticColumnMappings(IEnumerable<Field> fields,
            List<string> sourceNames) =>
            sourceNames.Count > 0 ?
                fields?.Where(e => ContainsName(sourceNames, e.Name)) :
                fields;

        /// <summary>
        /// StrictBypass: every source column must exist on the destination, in any order. The destination columns
        /// that are not supplied by the source are bypassed.
        /// </summary>
        private static List<Field> GetFieldsForStrictBypassColumnMappings(string tableName,
            List<Field> destinationFields,
            List<string> sourceNames)
        {
            var sourceColumnsNotInDestination = GetSourceColumnsNotInDestination(destinationFields, sourceNames);
            if (sourceColumnsNotInDestination.Count > 0)
            {
                throw new SqlServerBulkColumnMappingsException(tableName,
                    SqlServerBulkColumnMappingsBehavior.StrictBypass,
                    sourceColumnsNotInDestination,
                    null);
            }

            return destinationFields
                .Where(e => ContainsName(sourceNames, e.Name))
                .AsList();
        }

        /// <summary>
        /// Strict: the source columns must be identical to the destination columns, in both name and order.
        /// </summary>
        private static List<Field> GetFieldsForStrictColumnMappings(string tableName,
            List<Field> destinationFields,
            List<string> sourceNames)
        {
            var sourceColumnsNotInDestination = GetSourceColumnsNotInDestination(destinationFields, sourceNames);
            var destinationColumnsNotInSource = destinationFields
                .Where(e => !ContainsName(sourceNames, e.Name))
                .Select(e => e.Name)
                .AsList();

            if (sourceColumnsNotInDestination.Count > 0 || destinationColumnsNotInSource.Count > 0)
            {
                throw new SqlServerBulkColumnMappingsException(tableName,
                    SqlServerBulkColumnMappingsBehavior.Strict,
                    sourceColumnsNotInDestination,
                    destinationColumnsNotInSource);
            }

            var isOrderMismatch = sourceNames.Count != destinationFields.Count ||
                sourceNames.Where((name, index) => !string.Equals(name, destinationFields[index].Name, StringComparison.OrdinalIgnoreCase)).Any();
            if (isOrderMismatch)
            {
                throw new SqlServerBulkColumnMappingsException(tableName,
                    SqlServerBulkColumnMappingsBehavior.Strict,
                    null,
                    null,
                    true,
                    sourceNames,
                    destinationFields.Select(e => e.Name));
            }

            return destinationFields;
        }

        private static List<string> GetSourceColumnsNotInDestination(List<Field> destinationFields,
            List<string> sourceNames) =>
            sourceNames
                .Where(name => !destinationFields.Any(e => string.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase)))
                .AsList();

        private static bool ContainsName(List<string> names,
            string name) =>
            names.Any(e => string.Equals(e, name, StringComparison.OrdinalIgnoreCase));

        #endregion
    }
}
