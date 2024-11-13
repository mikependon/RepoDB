using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.PostgreSql.BulkOperations;

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
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            BulkImportIdentityBehavior identityBehavior,
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
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            Type entityType,
            BulkImportIdentityBehavior identityBehavior)
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
                identityBehavior);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="dictionaries"></param>
        /// <param name="mappings"></param>
        /// <param name="identityBehavior"></param>
        private static int BinaryImport(NpgsqlBinaryImporter importer,
            IEnumerable<IDictionary<string, object>> dictionaries,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            BulkImportIdentityBehavior identityBehavior)
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
                identityBehavior);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="rows"></param>
        /// <param name="mappings"></param>
        /// <param name="identityBehavior"></param>
        private static int BinaryImport(NpgsqlBinaryImporter importer,
            IEnumerable<DataRow> rows,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            BulkImportIdentityBehavior identityBehavior)
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
                identityBehavior);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="identityBehavior"></param>
        private static int BinaryImport(NpgsqlBinaryImporter importer,
            DbDataReader reader,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            BulkImportIdentityBehavior identityBehavior)
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
        /// <returns></returns>
        private static int BinaryImportWrite<TEntity>(NpgsqlBinaryImporter importer,
            Func<bool> moveNext,
            Func<TEntity> getCurrent,
            Action<TEntity> write,
            BulkImportIdentityBehavior identityBehavior)
            where TEntity : class
        {
            var result = 0;

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
                            data = Convert.ToInt32(data);
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
            BulkImportIdentityBehavior identityBehavior,
            int index)
        {
            if (identityBehavior == BulkImportIdentityBehavior.ReturnIdentity)
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
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            BulkImportIdentityBehavior identityBehavior,
            IDbSetting dbSetting,
            CancellationToken cancellationToken = default)
        {
            var copyCommand = GetBinaryImportCopyCommand(tableName,
                mappings,
                identityBehavior,
                dbSetting);

#if NET
            var importer = await connection.BeginBinaryImportAsync(copyCommand, cancellationToken);
#else
            var importer = await Task.FromResult(connection.BeginBinaryImport(copyCommand));
#endif

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
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            Type entityType,
            BulkImportIdentityBehavior identityBehavior,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var func = Compiler.GetNpgsqlBinaryImporterWriteAsyncFunc<TEntity>(tableName,
                mappings,
                entityType);
            var enumerator = entities.GetEnumerator();

            return await BinaryImportWriteAsync(importer,
                async () => await Task.FromResult(enumerator.MoveNext()),
                async () => await Task.FromResult(enumerator.Current),
                async (entity) => await func(importer, entity, cancellationToken),
                identityBehavior,
                cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="dictionaries"></param>
        /// <param name="mappings"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BinaryImportExplicitAsync(NpgsqlBinaryImporter importer,
            IEnumerable<IDictionary<string, object>> dictionaries,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            BulkImportIdentityBehavior identityBehavior,
            CancellationToken cancellationToken = default)
        {
            var enumerator = dictionaries.GetEnumerator();

            return await BinaryImportWriteAsync(importer,
                async () => await Task.FromResult(enumerator.MoveNext()),
                async () => await Task.FromResult(enumerator.Current),
                async (dictionary) =>
                {
                    foreach (var mapping in mappings)
                    {
                        await BinaryImportWriteAsync(importer, dictionary[mapping.SourceColumn],
                            mapping.NpgsqlDbType, cancellationToken);
                    }
                },
                identityBehavior,
                cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="rows"></param>
        /// <param name="mappings"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="cancellationToken"></param>
        private static async Task<int> BinaryImportAsync(NpgsqlBinaryImporter importer,
            IEnumerable<DataRow> rows,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            BulkImportIdentityBehavior identityBehavior,
            CancellationToken cancellationToken = default)
        {
            var enumerator = rows.GetEnumerator();

            return await BinaryImportWriteAsync(importer,
                async () => await Task.FromResult(enumerator.MoveNext()),
                async () => await Task.FromResult(enumerator.Current),
                async (row) =>
                {
                    foreach (var mapping in mappings)
                    {
                        var data = GetDataRowColumnData(row, mapping.SourceColumn, mapping.NpgsqlDbType);
                        await BinaryImportWriteAsync(importer, data, mapping.NpgsqlDbType, cancellationToken);
                    }
                },
                identityBehavior,
                cancellationToken);
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
            DbDataReader reader,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            BulkImportIdentityBehavior identityBehavior,
            CancellationToken cancellationToken = default)
        {
            return await BinaryImportWriteAsync(importer,
                async () => await reader.ReadAsync(cancellationToken),
                async () => await Task.FromResult(reader),
                async (current) =>
                {
                    foreach (var mapping in mappings)
                    {
                        await BinaryImportWriteAsync(importer, current[mapping.SourceColumn],
                            mapping.NpgsqlDbType, cancellationToken);
                    }
                },
                identityBehavior,
                cancellationToken: cancellationToken);
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
        /// <returns></returns>
        private static async Task<int> BinaryImportWriteAsync<TEntity>(NpgsqlBinaryImporter importer,
            Func<Task<bool>> moveNextAsync,
            Func<Task<TEntity>> getCurrentAsync,
            Func<TEntity, Task> writeAsync,
            BulkImportIdentityBehavior identityBehavior,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var result = 0;

            while (await moveNextAsync())
            {
                await importer.StartRowAsync(cancellationToken);

                await EnsureCustomizedOrderColumnAsync(importer, identityBehavior, result,
                    cancellationToken);
                await writeAsync(await getCurrentAsync());

                result++;
            }

            await importer.CompleteAsync(cancellationToken);
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
                await importer.WriteNullAsync(cancellationToken);
            }
            else
            {
                if (npgsqlDbType != null)
                {
                    await importer.WriteAsync(data, npgsqlDbType.Value, cancellationToken);
                }
                else
                {
                    await importer.WriteAsync(data, cancellationToken);
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
            BulkImportIdentityBehavior identityBehavior,
            int index,
            CancellationToken cancellationToken = default)
        {
            if (identityBehavior == BulkImportIdentityBehavior.ReturnIdentity)
            {
                await importer.WriteAsync(index, NpgsqlDbType.Integer, cancellationToken);
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
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            BulkImportIdentityBehavior identityBehavior,
            IDbSetting dbSetting)
        {
            if (identityBehavior == BulkImportIdentityBehavior.ReturnIdentity &&
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
        private static string GetTextColumns(IEnumerable<NpgsqlBulkInsertMapItem> mappings,
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
                    data = Convert.ToInt32(data);
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
