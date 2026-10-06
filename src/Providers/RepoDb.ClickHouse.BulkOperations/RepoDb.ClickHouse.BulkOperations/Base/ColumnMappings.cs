#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Diagnostics.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using RepoDb.DbSettings;
using RepoDb.Enumerations.ClickHouse;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb
{
    public static partial class ClickHouseConnectionExtension
    {
        #region ColumnMappings

        /// <summary>
        /// The name of the internal column that a data reader of the entities may expose to preserve the order of the rows.
        /// It is never a column of the source data.
        /// </summary>
        private const string OrderColumnName = "__RepoDb_OrderColumn";

        /// <summary>
        /// Validates the properties of the entities against the columns of the destination table, based on the
        /// <see cref="ClickHouseBulkDbSetting.BulkColumnMappingsBehavior"/> in used. It does nothing if explicit mappings were
        /// passed to the operation, as explicit mappings always take precedence over the behavior.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object, whose <see cref="ClickHouseBulkDbSetting"/> (if registered) defines the behavior.</param>
        /// <param name="tableName">The name of the destination table.</param>
        /// <param name="entities">The data entities (the source).</param>
        /// <param name="hasMappings">The value that indicates whether explicit mappings were passed to the operation.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        private static void ValidateColumnMappings<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            bool hasMappings,
            IDbTransaction transaction)
            where TEntity : class
        {
            var behavior = GetBulkColumnMappingsBehavior(connection, hasMappings);
            if (behavior == ClickHouseBulkColumnMappingsBehavior.Automatic || entities == null)
            {
                return;
            }

            using var reader = new DataEntityDataReader<TEntity>(entities);
            ValidateColumnMappings(connection, tableName, GetColumnNames(reader), behavior, transaction);
        }

        /// <summary>
        /// Validates the columns of the <see cref="DataTable"/> against the columns of the destination table, based on the
        /// <see cref="ClickHouseBulkDbSetting.BulkColumnMappingsBehavior"/> in used. It does nothing if explicit mappings were
        /// passed to the operation, as explicit mappings always take precedence over the behavior.
        /// </summary>
        /// <param name="connection">The connection object, whose <see cref="ClickHouseBulkDbSetting"/> (if registered) defines the behavior.</param>
        /// <param name="tableName">The name of the destination table.</param>
        /// <param name="table">The data table (the source).</param>
        /// <param name="hasMappings">The value that indicates whether explicit mappings were passed to the operation.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        private static void ValidateColumnMappings(IDbConnection connection,
            string tableName,
            DataTable table,
            bool hasMappings,
            IDbTransaction transaction)
        {
            var behavior = GetBulkColumnMappingsBehavior(connection, hasMappings);
            if (behavior == ClickHouseBulkColumnMappingsBehavior.Automatic || table == null)
            {
                return;
            }

            ValidateColumnMappings(connection, tableName, table.Columns.Cast<DataColumn>().Select(column => column.ColumnName), behavior, transaction);
        }

        /// <summary>
        /// Validates the fields of the <see cref="IDataReader"/> against the columns of the destination table, based on the
        /// <see cref="ClickHouseBulkDbSetting.BulkColumnMappingsBehavior"/> in used. It does nothing if explicit mappings were
        /// passed to the operation, as explicit mappings always take precedence over the behavior.
        /// </summary>
        /// <param name="connection">The connection object, whose <see cref="ClickHouseBulkDbSetting"/> (if registered) defines the behavior.</param>
        /// <param name="tableName">The name of the destination table.</param>
        /// <param name="reader">The data reader (the source).</param>
        /// <param name="hasMappings">The value that indicates whether explicit mappings were passed to the operation.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        private static void ValidateColumnMappings(IDbConnection connection,
            string tableName,
            IDataReader reader,
            bool hasMappings,
            IDbTransaction transaction)
        {
            var behavior = GetBulkColumnMappingsBehavior(connection, hasMappings);
            if (behavior == ClickHouseBulkColumnMappingsBehavior.Automatic || reader == null)
            {
                return;
            }

            ValidateColumnMappings(connection, tableName, GetColumnNames(reader), behavior, transaction);
        }

        /// <summary>
        /// Gets the column mappings behavior to be applied. Explicit mappings always take precedence over the behavior.
        /// </summary>
        private static ClickHouseBulkColumnMappingsBehavior GetBulkColumnMappingsBehavior(IDbConnection connection,
            bool hasMappings) =>
            hasMappings ?
                ClickHouseBulkColumnMappingsBehavior.Automatic :
                (connection.GetDbSetting() as ClickHouseBulkDbSetting)?.BulkColumnMappingsBehavior ?? default;

        /// <summary>
        /// Gets the names of the fields of the data reader, in order. The reader is not advanced.
        /// </summary>
        private static List<string> GetColumnNames(IDataReader reader) =>
            Enumerable.Range(0, reader.FieldCount)
                .Select(reader.GetName)
                .AsList();

        /// <summary>
        /// Validates the source columns against the columns of the destination table (in the order of the table columns).
        /// </summary>
        private static void ValidateColumnMappings(IDbConnection connection,
            string tableName,
            IEnumerable<string> sourceColumnNames,
            ClickHouseBulkColumnMappingsBehavior behavior,
            IDbTransaction transaction) =>
            ValidateColumnMappings(tableName,
                DbFieldCache.Get(connection, tableName, transaction),
                sourceColumnNames,
                connection.GetDbSetting(),
                behavior);

        /// <summary>
        /// Validates the source columns against the columns of the destination table, based on the given column mappings behavior.
        /// The validation is about the shape of the source only; which columns are written (i.e. the primary and identity columns)
        /// is still decided by the operation itself.
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
            ClickHouseBulkColumnMappingsBehavior behavior)
        {
            // Automatic: the columns that exist on both sides (the behavior of the earlier versions)
            if (behavior == ClickHouseBulkColumnMappingsBehavior.Automatic)
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

            if (behavior == ClickHouseBulkColumnMappingsBehavior.StrictBypass)
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
                throw new ClickHouseBulkColumnMappingsException(tableName,
                    ClickHouseBulkColumnMappingsBehavior.StrictBypass,
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
                throw new ClickHouseBulkColumnMappingsException(tableName,
                    ClickHouseBulkColumnMappingsBehavior.Strict,
                    sourceColumnsNotInDestination,
                    destinationColumnsNotInSource);
            }

            var isOrderMismatch = sourceNames.Count != destinationNames.Count ||
                sourceNames.Where((name, index) => !string.Equals(name, destinationNames[index], StringComparison.OrdinalIgnoreCase)).Any();
            if (isOrderMismatch)
            {
                throw new ClickHouseBulkColumnMappingsException(tableName,
                    ClickHouseBulkColumnMappingsBehavior.Strict,
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
