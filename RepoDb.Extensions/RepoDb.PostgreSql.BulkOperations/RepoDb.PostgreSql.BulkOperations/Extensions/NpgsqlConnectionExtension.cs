using Npgsql;
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
        /// <param name="entityType"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="keepIdentity"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static NpgsqlBinaryImporter GetNpgsqlBinaryImporter(NpgsqlConnection connection,
            string tableName,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            Type entityType = null,
            int? bulkCopyTimeout = null,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null)
        {
            var copyCommand = GetBinaryImportCopyCommand(connection,
                tableName,
                entityType,
                mappings,
                keepIdentity,
                transaction);
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
        /// <param name="connection"></param>
        /// <param name="importer"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="entityType"></param>
        /// <param name="keepIdentity"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BinaryImport<TEntity>(NpgsqlConnection connection,
            NpgsqlBinaryImporter importer,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            Type entityType = null,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null)
            where TEntity : class
        {
            if (mappings?.Any() == true)
            {
                return BinaryImport<TEntity>(importer,
                    tableName,
                    entities,
                    mappings,
                    entityType);
            }
            else
            {
                var dbFields = DbFieldCache.Get(connection,
                    (tableName ?? ClassMappedNameCache.Get(entityType)),
                    transaction);
                var properties = PropertyCache.Get(entityType);

                return BinaryImport<TEntity>(importer,
                    tableName,
                    entities,
                    dbFields,
                    properties,
                    entityType,
                    keepIdentity,
                    connection.GetDbSetting());
            }
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
        private static int BinaryImport<TEntity>(NpgsqlBinaryImporter importer,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            Type entityType)
            where TEntity : class
        {
            var func = Compiler.GetNpgsqlBinaryImporterWriteFunc<TEntity>(tableName,
                mappings,
                entityType);
            var enumerator = entities.GetEnumerator();

            return BinaryImportWrite(importer,
                () => enumerator.MoveNext(),
                () => enumerator.Current,
                (entity) => func(importer, entity));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="importer"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="dbFields"></param>
        /// <param name="properties"></param>
        /// <param name="entityType"></param>
        /// <param name="keepIdentity"></param>
        /// <param name="dbSetting"></param>
        private static int BinaryImport<TEntity>(NpgsqlBinaryImporter importer,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<DbField> dbFields,
            IEnumerable<ClassProperty> properties,
            Type entityType,
            bool keepIdentity = false,
            IDbSetting dbSetting = null)
            where TEntity : class
        {
            var func = Compiler.GetNpgsqlBinaryImporterWriteFunc<TEntity>(tableName,
                dbFields,
                properties,
                entityType,
                keepIdentity,
                dbSetting);
            var enumerator = entities.GetEnumerator();

            return BinaryImportWrite(importer,
                () => enumerator.MoveNext(),
                () => enumerator.Current,
                (entity) => func(importer, entity));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="dictionaries"></param>
        /// <param name="mappings"></param>
        private static int BinaryImportExplicit(NpgsqlBinaryImporter importer,
            IEnumerable<IDictionary<string, object>> dictionaries,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings)
        {
            var enumerator = dictionaries.GetEnumerator();

            return BinaryImportWrite(importer,
                () => enumerator.MoveNext(),
                () => enumerator.Current,
                (dictionary) =>
                {
                    foreach (var mapping in mappings)
                    {
                        importer.Write(dictionary[mapping.SourceColumn]);
                    }
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="rows"></param>
        /// <param name="mappings"></param>
        private static int BinaryImportExplicit(NpgsqlBinaryImporter importer,
            IEnumerable<DataRow> rows,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings)
        {
            var enumerator = rows.GetEnumerator();

            return BinaryImportWrite(importer,
                () => enumerator.MoveNext(),
                () => enumerator.Current,
                (row) =>
                {
                    foreach (var mapping in mappings)
                    {
                        importer.Write(row[mapping.SourceColumn]);
                    }
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        private static int BinaryImportExplicit(NpgsqlBinaryImporter importer,
            DbDataReader reader,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings)
        {
            return BinaryImportWrite(importer,
                () => reader.Read(),
                () => reader,
                (current) =>
                {
                    foreach (var mapping in mappings)
                    {
                        importer.Write(current[mapping.SourceColumn]);
                    }
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="importer"></param>
        /// <param name="moveNext"></param>
        /// <param name="getCurrent"></param>
        /// <param name="write"></param>
        /// <returns></returns>
        private static int BinaryImportWrite<TEntity>(NpgsqlBinaryImporter importer,
            Func<bool> moveNext,
            Func<TEntity> getCurrent,
            Action<TEntity> write)
            where TEntity : class
        {
            var result = 0;

            while (moveNext())
            {
                importer.StartRow();
                write(getCurrent());
                result++;
            }

            importer.Complete();
            return result;
        }

        #endregion

        #region Async

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="mappings"></param>
        /// <param name="entityType"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<NpgsqlBinaryImporter> GetNpgsqlBinaryImporterAsync(NpgsqlConnection connection,
            string tableName,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            Type entityType = null,
            int? bulkCopyTimeout = null,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var copyCommand = await GetBinaryImportCopyCommandAsync(connection,
                tableName,
                entityType,
                mappings,
                keepIdentity,
                transaction,
                cancellationToken);
#if NET6_0
            var importer = await connection.BeginBinaryImportAsync(copyCommand);
#else
            var importer = connection.BeginBinaryImport(copyCommand);
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
        /// <param name="connection"></param>
        /// <param name="importer"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="entityType"></param>
        /// <param name="keepIdentity"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BinaryImportAsync<TEntity>(NpgsqlConnection connection,
            NpgsqlBinaryImporter importer,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            Type entityType = null,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (mappings?.Any() == true)
            {
                return await BinaryImportAsync<TEntity>(importer,
                    tableName,
                    entities,
                    mappings,
                    entityType,
                    cancellationToken);
            }
            else
            {
                var dbFields = await DbFieldCache.GetAsync(connection,
                    (tableName ?? ClassMappedNameCache.Get(entityType)),
                    transaction,
                    cancellationToken);
                var properties = PropertyCache.Get(entityType);

                return await BinaryImportAsync<TEntity>(importer,
                    tableName,
                    entities,
                    dbFields,
                    properties,
                    entityType,
                    keepIdentity,
                    connection.GetDbSetting(),
                    cancellationToken);
            }
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
        /// <param name="cancellationToken"></param>
        private static async Task<int> BinaryImportAsync<TEntity>(NpgsqlBinaryImporter importer,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            Type entityType,
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
                cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="importer"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="dbFields"></param>
        /// <param name="properties"></param>
        /// <param name="entityType"></param>
        /// <param name="keepIdentity"></param>
        /// <param name="dbSetting"></param>
        /// <param name="cancellationToken"></param>
        private static async Task<int> BinaryImportAsync<TEntity>(NpgsqlBinaryImporter importer,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<DbField> dbFields,
            IEnumerable<ClassProperty> properties,
            Type entityType,
            bool keepIdentity = false,
            IDbSetting dbSetting = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var func = Compiler.GetNpgsqlBinaryImporterWriteAsyncFunc<TEntity>(tableName,
                dbFields,
                properties,
                entityType,
                keepIdentity,
                dbSetting);
            var enumerator = entities.GetEnumerator();

            return await BinaryImportWriteAsync(importer,
                async () => await Task.FromResult(enumerator.MoveNext()),
                async () => await Task.FromResult(enumerator.Current),
                async (entity) => await func(importer, entity, cancellationToken),
                cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="dictionaries"></param>
        /// <param name="mappings"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BinaryImportExplicitAsync(NpgsqlBinaryImporter importer,
            IEnumerable<IDictionary<string, object>> dictionaries,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
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
                        await importer.WriteAsync(dictionary[mapping.SourceColumn], cancellationToken);
                    }
                },
                cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="rows"></param>
        /// <param name="mappings"></param>
        /// <param name="cancellationToken"></param>
        private static async Task<int> BinaryImportExplicitAsync(NpgsqlBinaryImporter importer,
            IEnumerable<DataRow> rows,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
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
                        await importer.WriteAsync(row[mapping.SourceColumn], cancellationToken);
                    }
                },
                cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="cancellationToken"></param>
        private static async Task<int> BinaryImportExplicitAsync(NpgsqlBinaryImporter importer,
            DbDataReader reader,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            CancellationToken cancellationToken = default)
        {
            return await BinaryImportWriteAsync(importer,
                async () => await reader.ReadAsync(cancellationToken),
                async () => await Task.FromResult(reader),
                async (current) =>
                {
                    foreach (var mapping in mappings)
                    {
                        await importer.WriteAsync(current[mapping.SourceColumn], cancellationToken);
                    }
                },
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BinaryImportWriteAsync<TEntity>(NpgsqlBinaryImporter importer,
            Func<Task<bool>> moveNextAsync,
            Func<Task<TEntity>> getCurrentAsync,
            Func<TEntity, Task> writeAsync,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var result = 0;

            while (await moveNextAsync())
            {
                await importer.StartRowAsync(cancellationToken);
                await writeAsync(await getCurrentAsync());
                result++;
            }

            await importer.CompleteAsync(cancellationToken);
            return result;
        }

        #endregion

        #region Others

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entityType"></param>
        /// <param name="mappings"></param>
        /// <param name="keepIdentity"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static string GetBinaryImportCopyCommand(NpgsqlConnection connection,
            string tableName,
            Type entityType,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null)
        {
            var targetTableName = (tableName ?? ClassMappedNameCache.Get(entityType)).AsQuoted(true, connection.GetDbSetting());
            var textColumns = GetTextColumns(connection,
                tableName,
                entityType,
                mappings,
                keepIdentity,
                transaction);

            return $"COPY {targetTableName} ({textColumns}) FROM STDIN (FORMAT BINARY)";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entityType"></param>
        /// <param name="mappings"></param>
        /// <param name="keepIdentity"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<string> GetBinaryImportCopyCommandAsync(NpgsqlConnection connection,
            string tableName,
            Type entityType,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var targetTableName = (tableName ?? ClassMappedNameCache.Get(entityType)).AsQuoted(true, connection.GetDbSetting());
            var textColumns = await GetTextColumnsAsync(connection,
                tableName,
                entityType,
                mappings,
                keepIdentity,
                transaction,
                cancellationToken);

            return $"COPY {targetTableName} ({textColumns}) FROM STDIN (FORMAT BINARY)";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entityType"></param>
        /// <param name="mappings"></param>
        /// <param name="keepIdentity"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static string GetTextColumns(NpgsqlConnection connection,
            string tableName,
            Type entityType,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null)
        {
            var dbSetting = connection.GetDbSetting();

            if (mappings?.Any() == true)
            {
                return GetTextColumns(mappings, dbSetting);
            }
            else
            {
                var dbFields = DbFieldCache.Get(connection,
                    (tableName ?? ClassMappedNameCache.Get(entityType)),
                    transaction);
                return GetTextColumns(dbFields, PropertyCache.Get(entityType), keepIdentity, dbSetting);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entityType"></param>
        /// <param name="mappings"></param>
        /// <param name="keepIdentity"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<string> GetTextColumnsAsync(NpgsqlConnection connection,
            string tableName,
            Type entityType,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            bool keepIdentity = false,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbSetting = connection.GetDbSetting();

            if (mappings?.Any() == true)
            {
                return GetTextColumns(mappings, dbSetting);
            }
            else
            {
                var dbFields = await DbFieldCache.GetAsync(connection,
                    (tableName ?? ClassMappedNameCache.Get(entityType)),
                    transaction,
                    cancellationToken);
                return GetTextColumns(dbFields, PropertyCache.Get(entityType), keepIdentity, dbSetting);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mappings"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetTextColumns(IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            IDbSetting dbSetting) =>
            mappings.Select(mapping => mapping.DestinationColumn.AsQuoted(true, dbSetting)).Join(", ");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbFields"></param>
        /// <param name="properties"></param>
        /// <param name="keepIdentity"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetTextColumns(IEnumerable<DbField> dbFields,
            IEnumerable<ClassProperty> properties,
            bool keepIdentity = false,
            IDbSetting dbSetting = null) =>
            GetMatchedProperties(dbFields, properties, keepIdentity, dbSetting)
                .Select(property => property.GetMappedName().AsQuoted(true, dbSetting)).Join(", ");

        #endregion
    }
}
