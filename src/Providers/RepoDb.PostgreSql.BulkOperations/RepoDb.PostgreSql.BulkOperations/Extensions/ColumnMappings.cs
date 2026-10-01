#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.DbSettings;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb
{
    public static partial class NpgsqlConnectionExtension
    {
        #region ColumnMappings

        /// <summary>
        /// The name of the internal column that the bulk operations use to preserve the order of the rows. It is never a
        /// column of the source data.
        /// </summary>
        private const string OrderColumnName = "__RepoDb_OrderColumn";

        /// <summary>
        /// Validates the source columns against the columns of the destination table, based on the
        /// <see cref="PostgreSqlBulkOperationsDbSetting.BulkColumnMappingsBehavior"/> of the given setting. This is only called
        /// when no explicit mappings were passed to the operation, as explicit mappings always take precedence over the behavior.
        /// The validation is about the shape of the source only; which columns are written (i.e. the primary and identity columns)
        /// is still decided by the operation itself.
        /// </summary>
        /// <param name="tableName">The name of the destination table.</param>
        /// <param name="dbFields">The fields of the destination table, in the order of the table columns.</param>
        /// <param name="sourceColumnNames">The names of the source columns, in order.</param>
        /// <param name="dbSetting">The setting of the connection (a <see cref="PostgreSqlBulkOperationsDbSetting"/> defines the behavior).</param>
        private static void ValidateColumnMappings(string tableName,
            DbFieldCollection dbFields,
            IEnumerable<string> sourceColumnNames,
            IDbSetting dbSetting) =>
            ValidateColumnMappings(tableName,
                dbFields,
                sourceColumnNames,
                dbSetting,
                (dbSetting as PostgreSqlBulkOperationsDbSetting)?.BulkColumnMappingsBehavior ?? default);

        /// <summary>
        /// Validates the source columns against the columns of the destination table, based on the given column mappings behavior.
        /// </summary>
        /// <param name="tableName">The name of the destination table.</param>
        /// <param name="dbFields">The fields of the destination table, in the order of the table columns.</param>
        /// <param name="sourceColumnNames">The names of the source columns, in order.</param>
        /// <param name="dbSetting">The setting of the connection.</param>
        /// <param name="behavior">The column mappings behavior to be applied.</param>
        internal static void ValidateColumnMappings(string tableName,
            DbFieldCollection dbFields,
            IEnumerable<string> sourceColumnNames,
            IDbSetting dbSetting,
            PostgreSqlBulkColumnMappingsBehavior behavior)
        {
            // Automatic: the columns that exist on both sides (the behavior of the earlier versions)
            if (behavior == PostgreSqlBulkColumnMappingsBehavior.Automatic)
            {
                return;
            }

            var sourceNames = sourceColumnNames?
                .Select(name => name.AsUnquoted(true, dbSetting))
                .Where(name => !string.Equals(name, OrderColumnName, StringComparison.OrdinalIgnoreCase))
                .AsList() ?? [];
            var destinationNames = dbFields?
                .GetItems()
                .Select(dbField => dbField.Name.AsUnquoted(true, dbSetting))
                .AsList() ?? [];
            var sourceColumnsNotInDestination = sourceNames
                .Where(name => !ContainsName(destinationNames, name))
                .AsList();

            if (behavior == PostgreSqlBulkColumnMappingsBehavior.StrictBypass)
            {
                ValidateStrictBypassColumnMappings(tableName, sourceColumnsNotInDestination);
            }
            else
            {
                ValidateStrictColumnMappings(tableName, sourceNames, destinationNames, sourceColumnsNotInDestination);
            }
        }

        /// <summary>
        /// StrictBypass: every source column must exist on the destination, in any order. The destination columns
        /// that are not supplied by the source are bypassed.
        /// </summary>
        private static void ValidateStrictBypassColumnMappings(string tableName,
            List<string> sourceColumnsNotInDestination)
        {
            if (sourceColumnsNotInDestination.Count > 0)
            {
                throw new PostgreSqlBulkColumnMappingsException(tableName,
                    PostgreSqlBulkColumnMappingsBehavior.StrictBypass,
                    sourceColumnsNotInDestination,
                    null);
            }
        }

        /// <summary>
        /// Strict: the source columns must be identical to the destination columns, in both name and order.
        /// </summary>
        private static void ValidateStrictColumnMappings(string tableName,
            List<string> sourceNames,
            List<string> destinationNames,
            List<string> sourceColumnsNotInDestination)
        {
            var destinationColumnsNotInSource = destinationNames
                .Where(name => !ContainsName(sourceNames, name))
                .AsList();

            if (sourceColumnsNotInDestination.Count > 0 || destinationColumnsNotInSource.Count > 0)
            {
                throw new PostgreSqlBulkColumnMappingsException(tableName,
                    PostgreSqlBulkColumnMappingsBehavior.Strict,
                    sourceColumnsNotInDestination,
                    destinationColumnsNotInSource);
            }

            var isOrderMismatch = sourceNames.Count != destinationNames.Count ||
                sourceNames.Where((name, index) => !string.Equals(name, destinationNames[index], StringComparison.OrdinalIgnoreCase)).Any();
            if (isOrderMismatch)
            {
                throw new PostgreSqlBulkColumnMappingsException(tableName,
                    PostgreSqlBulkColumnMappingsBehavior.Strict,
                    null,
                    null,
                    true,
                    sourceNames,
                    destinationNames);
            }
        }

        private static bool ContainsName(List<string> names,
            string name) =>
            names.Any(e => string.Equals(e, name, StringComparison.OrdinalIgnoreCase));

        #endregion
    }
}
