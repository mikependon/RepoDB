#region Copyright Attributions

// Copyright (c) 2021 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Npgsql;
using NpgsqlTypes;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.PostgreSql.BulkOperations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for NpgsqlConnection object.
    /// </summary>
    public static partial class NpgsqlConnectionExtension
    {
        #region Sync

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static NpgsqlBinaryImporter GetNpgsqlBinaryImporter(NpgsqlConnection connection,
            string tableName,
            IEnumerable<PostgreSqlBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            PostgreSqlBulkImportIdentityBehavior identityBehavior,
            IDbSetting dbSetting)
        {
            var copyCommand = GetBinaryImportCopyCommand(tableName,
                mappings,
                identityBehavior,
                dbSetting);
            var importer = connection.BeginBinaryImport(copyCommand);

            // Timeout
            if (bulkCopyTimeout.HasValue)
            {
                importer.Timeout = TimeSpan.FromSeconds(bulkCopyTimeout.Value);
            }

            // Return
            return importer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="importer"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="entityType"></param>
        /// <param name="identityBehavior"></param>
        private static int BinaryImport<TEntity>(NpgsqlBinaryImporter importer,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<PostgreSqlBulkInsertMapItem> mappings,
            Type entityType,
            PostgreSqlBulkImportIdentityBehavior identityBehavior,
            int startIndex = 0)
            where TEntity : class
        {
            var func = Compiler.GetNpgsqlBinaryImporterWriteFunc<TEntity>(tableName,
                mappings,
                entityType);
            var enumerator = entities.GetEnumerator();

            return BinaryImportWrite(importer,
                () => enumerator.MoveNext(),
                () => enumerator.Current,
                (entity) => func(importer, entity),
                identityBehavior,
                startIndex);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="dictionaries"></param>
        /// <param name="mappings"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="startIndex"></param>
        private static int BinaryImport(NpgsqlBinaryImporter importer,
            IEnumerable<IDictionary<string, object>> dictionaries,
            IEnumerable<PostgreSqlBulkInsertMapItem> mappings,
            PostgreSqlBulkImportIdentityBehavior identityBehavior,
            int startIndex = 0)
        {
            var enumerator = dictionaries.GetEnumerator();

            return BinaryImportWrite(importer,
                () => enumerator.MoveNext(),
                () => enumerator.Current,
                (dictionary) =>
                {
                    foreach (var mapping in mappings)
                    {
                        BinaryImportWrite(importer, dictionary[mapping.SourceColumn], mapping.NpgsqlDbType);
                    }
                },
                identityBehavior,
                startIndex);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="rows"></param>
        /// <param name="mappings"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="startIndex"></param>
        private static int BinaryImport(NpgsqlBinaryImporter importer,
            IEnumerable<DataRow> rows,
            IEnumerable<PostgreSqlBulkInsertMapItem> mappings,
            PostgreSqlBulkImportIdentityBehavior identityBehavior,
            int startIndex = 0)
        {
            var enumerator = rows.GetEnumerator();

            return BinaryImportWrite(importer,
                () => enumerator.MoveNext(),
                () => enumerator.Current,
                (row) =>
                {
                    foreach (var mapping in mappings)
                    {
                        var data = GetDataRowColumnData(row, mapping.SourceColumn, mapping.NpgsqlDbType);
                        BinaryImportWrite(importer, data, mapping.NpgsqlDbType);
                    }
                },
                identityBehavior,
                startIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="identityBehavior"></param>
        private static int BinaryImport(NpgsqlBinaryImporter importer,
            IDataReader reader,
            IEnumerable<PostgreSqlBulkInsertMapItem> mappings,
            PostgreSqlBulkImportIdentityBehavior identityBehavior)
        {
            return BinaryImportWrite(importer,
                () => reader.Read(),
                () => reader,
                (current) =>
                {
                    foreach (var mapping in mappings)
                    {
                        BinaryImportWrite(importer, current[mapping.SourceColumn], mapping.NpgsqlDbType);
                    }
                },
                identityBehavior);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="importer"></param>
        /// <param name="moveNext"></param>
        /// <param name="getCurrent"></param>
        /// <param name="write"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="startIndex">The running __RepoDb_OrderColumn index to continue from (carries over across batches so batched
        /// imports keep a single, globally-ordered sequence instead of each batch restarting at 0).</param>
        /// <returns></returns>
        private static int BinaryImportWrite<TEntity>(NpgsqlBinaryImporter importer,
            Func<bool> moveNext,
            Func<TEntity> getCurrent,
            Action<TEntity> write,
            PostgreSqlBulkImportIdentityBehavior identityBehavior,
            int startIndex = 0)
            where TEntity : class
        {
            var result = startIndex;

            while (moveNext())
            {
                importer.StartRow();

                EnsureCustomizedOrderColumn(importer, identityBehavior, result);
                write(getCurrent());

                result++;
            }

            importer.Complete();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="data"></param>
        /// <param name="npgsqlDbType"></param>
        private static void BinaryImportWrite(NpgsqlBinaryImporter importer,
            object data,
            NpgsqlDbType? npgsqlDbType)
        {
            if (data == null)
            {
                importer.WriteNull();
            }
            else
            {
                if (npgsqlDbType != null)
                {
                    if (data is Enum)
                    {
                        if (npgsqlDbType == NpgsqlDbType.Integer ||
                            npgsqlDbType == NpgsqlDbType.Bigint ||
                            npgsqlDbType == NpgsqlDbType.Smallint)
                        {
                            data = Convert.ToInt32(data, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        else if (npgsqlDbType == NpgsqlDbType.Text)
                        {
                            data = Convert.ToString(data);
                        }
                    }
                    importer.Write(data, npgsqlDbType.Value);
                }
                else
                {
                    importer.Write(data);
                }
            }
        }

        /// <summary>
        /// For __RepoDb_OrderColumn
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="index"></param>
        private static void EnsureCustomizedOrderColumn(NpgsqlBinaryImporter importer,
            PostgreSqlBulkImportIdentityBehavior identityBehavior,
            int index)
        {
            if (identityBehavior == PostgreSqlBulkImportIdentityBehavior.ReturnIdentity)
            {
                importer.Write(index, NpgsqlDbType.Integer);
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="dbSetting"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<NpgsqlBinaryImporter> GetNpgsqlBinaryImporterAsync(NpgsqlConnection connection,
            string tableName,
            IEnumerable<PostgreSqlBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            PostgreSqlBulkImportIdentityBehavior identityBehavior,
            IDbSetting dbSetting,
            CancellationToken cancellationToken = default)
        {
            var copyCommand = GetBinaryImportCopyCommand(tableName,
                mappings,
                identityBehavior,
                dbSetting);

            var importer = await connection.BeginBinaryImportAsync(copyCommand, cancellationToken).ConfigureAwait(false);

            // Timeout
            if (bulkCopyTimeout.HasValue)
            {
                importer.Timeout = TimeSpan.FromSeconds(bulkCopyTimeout.Value);
            }

            // Return
            return importer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="importer"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="entityType"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="cancellationToken"></param>
        private static async Task<int> BinaryImportAsync<TEntity>(NpgsqlBinaryImporter importer,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<PostgreSqlBulkInsertMapItem> mappings,
            Type entityType,
            PostgreSqlBulkImportIdentityBehavior identityBehavior,
            CancellationToken cancellationToken = default,
            int startIndex = 0)
            where TEntity : class
        {
            var func = Compiler.GetNpgsqlBinaryImporterWriteAsyncFunc<TEntity>(tableName,
                mappings,
                entityType);
            var enumerator = entities.GetEnumerator();

            return await BinaryImportWriteAsync(importer,
                async () => await Task.FromResult(enumerator.MoveNext()).ConfigureAwait(false),
                async () => await Task.FromResult(enumerator.Current).ConfigureAwait(false),
                async (entity) => await func(importer, entity, cancellationToken).ConfigureAwait(false),
                identityBehavior,
                cancellationToken,
                startIndex).ConfigureAwait(false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="dictionaries"></param>
        /// <param name="mappings"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private static async Task<int> BinaryImportExplicitAsync(NpgsqlBinaryImporter importer,
            IEnumerable<IDictionary<string, object>> dictionaries,
            IEnumerable<PostgreSqlBulkInsertMapItem> mappings,
            PostgreSqlBulkImportIdentityBehavior identityBehavior,
            CancellationToken cancellationToken = default,
            int startIndex = 0)
        {
            var enumerator = dictionaries.GetEnumerator();

            return await BinaryImportWriteAsync(importer,
                async () => await Task.FromResult(enumerator.MoveNext()).ConfigureAwait(false),
                async () => await Task.FromResult(enumerator.Current).ConfigureAwait(false),
                async (dictionary) =>
                {
                    foreach (var mapping in mappings)
                    {
                        await BinaryImportWriteAsync(importer, dictionary[mapping.SourceColumn],
                            mapping.NpgsqlDbType, cancellationToken).ConfigureAwait(false);
                    }
                },
                identityBehavior,
                cancellationToken,
                startIndex).ConfigureAwait(false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="rows"></param>
        /// <param name="mappings"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="startIndex"></param>
        private static async Task<int> BinaryImportAsync(NpgsqlBinaryImporter importer,
            IEnumerable<DataRow> rows,
            IEnumerable<PostgreSqlBulkInsertMapItem> mappings,
            PostgreSqlBulkImportIdentityBehavior identityBehavior,
            CancellationToken cancellationToken = default,
            int startIndex = 0)
        {
            var enumerator = rows.GetEnumerator();

            return await BinaryImportWriteAsync(importer,
                async () => await Task.FromResult(enumerator.MoveNext()).ConfigureAwait(false),
                async () => await Task.FromResult(enumerator.Current).ConfigureAwait(false),
                async (row) =>
                {
                    foreach (var mapping in mappings)
                    {
                        var data = GetDataRowColumnData(row, mapping.SourceColumn, mapping.NpgsqlDbType);
                        await BinaryImportWriteAsync(importer, data, mapping.NpgsqlDbType, cancellationToken).ConfigureAwait(false);
                    }
                },
                identityBehavior,
                cancellationToken,
                startIndex).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="cancellationToken"></param>
        private static async Task<int> BinaryImportAsync(NpgsqlBinaryImporter importer,
            IDataReader reader,
            IEnumerable<PostgreSqlBulkInsertMapItem> mappings,
            PostgreSqlBulkImportIdentityBehavior identityBehavior,
            CancellationToken cancellationToken = default)
        {
            return await BinaryImportWriteAsync(importer,
                async () => await (reader is DbDataReader r ? r.ReadAsync(cancellationToken) : Task.FromResult(reader.Read())).ConfigureAwait(false),
                async () => await Task.FromResult(reader).ConfigureAwait(false),
                async (current) =>
                {
                    foreach (var mapping in mappings)
                    {
                        await BinaryImportWriteAsync(importer, current[mapping.SourceColumn],
                            mapping.NpgsqlDbType, cancellationToken).ConfigureAwait(false);
                    }
                },
                identityBehavior,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="importer"></param>
        /// <param name="moveNextAsync"></param>
        /// <param name="getCurrentAsync"></param>
        /// <param name="writeAsync"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="startIndex">The running __RepoDb_OrderColumn index to continue from (carries over across batches so batched
        /// imports keep a single, globally-ordered sequence instead of each batch restarting at 0).</param>
        /// <returns></returns>
        private static async Task<int> BinaryImportWriteAsync<TEntity>(NpgsqlBinaryImporter importer,
            Func<Task<bool>> moveNextAsync,
            Func<Task<TEntity>> getCurrentAsync,
            Func<TEntity, Task> writeAsync,
            PostgreSqlBulkImportIdentityBehavior identityBehavior,
            CancellationToken cancellationToken = default,
            int startIndex = 0)
            where TEntity : class
        {
            var result = startIndex;

            while (await moveNextAsync().ConfigureAwait(false))
            {
                await importer.StartRowAsync(cancellationToken).ConfigureAwait(false);

                await EnsureCustomizedOrderColumnAsync(importer, identityBehavior, result,
                    cancellationToken).ConfigureAwait(false);
                await writeAsync(await getCurrentAsync().ConfigureAwait(false)).ConfigureAwait(false);

                result++;
            }

            await importer.CompleteAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="data"></param>
        /// <param name="npgsqlDbType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task BinaryImportWriteAsync(NpgsqlBinaryImporter importer,
            object data,
            NpgsqlDbType? npgsqlDbType,
            CancellationToken cancellationToken = default)
        {
            if (data == null)
            {
                await importer.WriteNullAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                if (npgsqlDbType != null)
                {
                    await importer.WriteAsync(data, npgsqlDbType.Value, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    await importer.WriteAsync(data, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// For __RepoDb_OrderColumn
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="index"></param>
        /// <param name="cancellationToken"></param>
        private static async Task EnsureCustomizedOrderColumnAsync(NpgsqlBinaryImporter importer,
            PostgreSqlBulkImportIdentityBehavior identityBehavior,
            int index,
            CancellationToken cancellationToken = default)
        {
            if (identityBehavior == PostgreSqlBulkImportIdentityBehavior.ReturnIdentity)
            {
                await importer.WriteAsync(index, NpgsqlDbType.Integer, cancellationToken).ConfigureAwait(false);
            }

        }

        #endregion

        #region Others

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="mappings"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetBinaryImportCopyCommand(string tableName,
            IEnumerable<PostgreSqlBulkInsertMapItem> mappings,
            PostgreSqlBulkImportIdentityBehavior identityBehavior,
            IDbSetting dbSetting)
        {
            if (identityBehavior == PostgreSqlBulkImportIdentityBehavior.ReturnIdentity &&
                mappings.FirstOrDefault(mapping =>
                    string.Equals(mapping.DestinationColumn, "__RepoDb_OrderColumn", StringComparison.OrdinalIgnoreCase)) == null)
            {
                mappings = AddOrderColumnMapping(mappings);
            }

            var textColumns = GetTextColumns(mappings, dbSetting);

            return $"COPY {tableName.AsQuoted(true, dbSetting)} ({textColumns}) FROM STDIN (FORMAT BINARY)";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mappings"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetTextColumns(IEnumerable<PostgreSqlBulkInsertMapItem> mappings,
            IDbSetting dbSetting) =>
            mappings?.Select(mapping => mapping.DestinationColumn.AsQuoted(true, dbSetting)).Join(", ");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object GetDataRowColumnData(DataRow row,
            string columnName,
            NpgsqlDbType? type)
        {
            var columnType = row.Table.Columns[columnName].DataType;
            var data = row[columnName];
            if (columnType.IsEnum)
            {
                if (type == NpgsqlDbType.Integer ||
                    type == NpgsqlDbType.Bigint ||
                    type == NpgsqlDbType.Smallint)
                {
                    data = Convert.ToInt32(data, System.Globalization.CultureInfo.InvariantCulture);
                }
                else if (type == NpgsqlDbType.Text)
                {
                    data = Convert.ToString(data);
                }
                else
                {
                    if (!(data is Enum) && data != DBNull.Value)
                    {
                        data = Enum.ToObject(columnType, data);
                    }
                }
            }
            return data;
        }

        #endregion
    }
}
