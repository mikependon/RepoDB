using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using RepoDb;
using RepoDb.Enumerations.MariaDb;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.MariaDb.BulkOperations;

namespace RepoDb
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class MariaDbConnectionExtension
    {
        #region WriteToServerInternal

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="transaction"></param>
        /// <param name="excludeField"></param>
        /// <returns></returns>
        internal static int WriteToServerInternal<TEntity>(MySqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<MariaDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MySqlTransaction transaction = null,
            Field excludeField = null)
            where TEntity : class
        {
            connection.EnsureOpen();
            using var reader = new DataEntityDataReader<TEntity>(entities);
            var (bulkCopy, filteredReader) = CreateBulkCopyForDataReader(connection, tableName, reader, mappings, bulkCopyTimeout, transaction, excludeField);
            bulkCopy.WriteToServer(filteredReader);
            return entities != null ? entities.Count() : 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="excludeField"></param>
        /// <returns></returns>
        internal static int WriteToServerInternal(MySqlConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<MariaDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Field excludeField = null)
        {
            connection.EnsureOpen();
            var bulkCopy = CreateBulkCopyForDataTable(connection, tableName, table, mappings, bulkCopyTimeout, excludeField);
            var rows = GetDataRows(table, rowState)?.ToArray();
            bulkCopy.WriteToServer(rows, table.Columns.Count);
            return rows != null ? rows.Length : 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="transaction"></param>
        /// <param name="excludeField"></param>
        /// <returns></returns>
        internal static int WriteToServerInternal(MySqlConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<MariaDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MySqlTransaction transaction = null,
            Field excludeField = null)
        {
            connection.EnsureOpen();
            var countingReader = new CountingDataReader(reader);
            var (bulkCopy, filteredReader) = CreateBulkCopyForDataReader(connection, tableName, countingReader, mappings, bulkCopyTimeout, transaction, excludeField);
            bulkCopy.WriteToServer(filteredReader);
            return countingReader.Count;
        }

        #endregion

        #region WriteToServerAsyncInternal

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="transaction"></param>
        /// <param name="excludeField"></param>
        /// <returns></returns>
        internal static async Task<int> WriteToServerAsyncInternal<TEntity>(MySqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<MariaDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CancellationToken cancellationToken = default,
            MySqlTransaction transaction = null,
            Field excludeField = null)
            where TEntity : class
        {
            await connection.EnsureOpenAsync(cancellationToken);
            using var reader = new DataEntityDataReader<TEntity>(entities);
            var (bulkCopy, filteredReader) = CreateBulkCopyForDataReader(connection, tableName, reader, mappings, bulkCopyTimeout, transaction, excludeField);
            await bulkCopy.WriteToServerAsync(filteredReader, cancellationToken);
            return entities != null ? entities.Count() : 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="excludeField"></param>
        /// <returns></returns>
        internal static async Task<int> WriteToServerAsyncInternal(MySqlConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<MariaDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CancellationToken cancellationToken = default,
            Field excludeField = null)
        {
            await connection.EnsureOpenAsync(cancellationToken);
            var bulkCopy = CreateBulkCopyForDataTable(connection, tableName, table, mappings, bulkCopyTimeout, excludeField);
            var rows = GetDataRows(table, rowState)?.ToArray();
            await bulkCopy.WriteToServerAsync(rows, table.Columns.Count, cancellationToken);
            return rows != null ? rows.Length : 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="transaction"></param>
        /// <param name="excludeField"></param>
        /// <returns></returns>
        internal static async Task<int> WriteToServerAsyncInternal(MySqlConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<MariaDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CancellationToken cancellationToken = default,
            MySqlTransaction transaction = null,
            Field excludeField = null)
        {
            await connection.EnsureOpenAsync(cancellationToken);
            return await Task.Run(() => // No underlying 'Async' equivalent for 'WriteToServerInternal'
            {
                var countingReader = new CountingDataReader(reader);
                var (bulkCopy, filteredReader) = CreateBulkCopyForDataReader(connection, tableName, countingReader, mappings, bulkCopyTimeout, transaction, excludeField);
                bulkCopy.WriteToServer(filteredReader);
                return countingReader.Count;
            },
            cancellationToken);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pseudoTableType"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        private static MariaDbBulkImportPseudoTableType ResolvePseudoTableType(MariaDbBulkImportPseudoTableType pseudoTableType,
            int? rowCount) =>
            pseudoTableType == MariaDbBulkImportPseudoTableType.Auto && rowCount.GetValueOrDefault() >= MariaDbConstants.RowCountThresholdForPhysicalTable ?
                MariaDbBulkImportPseudoTableType.Physical :
                    MariaDbBulkImportPseudoTableType.Physical;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="rowState"></param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        private static IEnumerable<DataRow> GetDataRows(DataTable dataTable,
            DataRowState? rowState = null)
        {
            var rows = dataTable.Rows.OfType<DataRow>();
            if (rowState.HasValue == true)
            {
                rows = rows.Where(r => r.RowState == rowState);
            }
            if (!rows.Any())
            {
                throw new System.InvalidOperationException($"No rows found from data table where the state is '{rowState.ToString()}'.");
            }
            foreach (var row in rows)
            {
                yield return row;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="excludeField"></param>
        /// <returns></returns>
        /// <exception cref="InvalidTypeException"></exception>
        private static MariaDbBulkCopy CreateBulkCopyForDataTable(MySqlConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<MariaDbBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            Field excludeField = null)
        {
            var dbSetting = connection.GetDbSetting();
            var bulkCopy = new MariaDbBulkCopy(connection)
            {
                DestinationTableName = tableName.AsQuoted(true, dbSetting)
            };
            if (bulkCopyTimeout.HasValue)
            {
                bulkCopy.BulkCopyTimeout = bulkCopyTimeout.Value;
            }
            if (mappings != null)
            {
                var dbFields = DbFieldCache.Get(connection, tableName, null);
                var columnMappings = mappings.AsList();
                foreach (var mapping in columnMappings)
                {
                    var sourceOrdinal = table.Columns.IndexOf(mapping.SourceColumn);
                    if (sourceOrdinal < 0)
                    {
                        throw new InvalidTypeException($"The source column '{mapping.SourceColumn}' defined in the mappings was not found in the given data table.");
                    }
                    var destinationField = dbFields?.GetByUnquotedName(mapping.DestinationColumn.AsUnquoted(true, dbSetting));
                    var sourceType = table.Columns[sourceOrdinal].DataType;
                    if (destinationField?.Type != null && sourceType != null && !AreMappingTypesCompatible(sourceType, destinationField.Type))
                    {
                        throw new InvalidTypeException($"The type of the source column '{mapping.SourceColumn}' ({sourceType}) does not match the type of the destination column '{mapping.DestinationColumn}' ({destinationField.Type}).");
                    }
                    bulkCopy.ColumnMappings.Add(
                        new MariaDbBulkCopyColumnMapping(sourceOrdinal, mapping.DestinationColumn));
                }
            }
            else
            {
                foreach (DataColumn column in table.Columns)
                {
                    if (excludeField != null && string.Equals(column.ColumnName, excludeField.Name, System.StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    bulkCopy.ColumnMappings.Add(
                        new MariaDbBulkCopyColumnMapping(table.Columns.IndexOf(column), column.ColumnName));
                }
            }
            return bulkCopy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        private static bool AreMappingTypesCompatible(Type sourceType,
            Type destinationType)
        {
            sourceType = Nullable.GetUnderlyingType(sourceType) ?? sourceType;
            destinationType = Nullable.GetUnderlyingType(destinationType) ?? destinationType;

            if (sourceType == destinationType)
            {
                return true;
            }
            if ((sourceType == typeof(Guid) && destinationType == typeof(string)) ||
                (sourceType == typeof(string) && destinationType == typeof(Guid)))
            {
                return true;
            }

            static bool IsIntegral(Type type)
            {
                var code = Type.GetTypeCode(type);
                return code is TypeCode.SByte or TypeCode.Byte or TypeCode.Int16 or TypeCode.UInt16
                    or TypeCode.Int32 or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64;
            }

            return IsIntegral(sourceType) && IsIntegral(destinationType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="excludeField"></param>
        /// <returns></returns>
        /// <exception cref="InvalidTypeException"></exception>
        private static (MariaDbBulkCopy BulkCopy, IDataReader Reader) CreateBulkCopyForDataReader(MySqlConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<MariaDbBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            MySqlTransaction transaction,
            Field excludeField = null)
        {
            var dbSetting = connection.GetDbSetting();
            var bulkCopy = new MariaDbBulkCopy(connection)
            {
                DestinationTableName = tableName.AsQuoted(true, dbSetting)
            };
            if (bulkCopyTimeout.HasValue)
            {
                bulkCopy.BulkCopyTimeout = bulkCopyTimeout.Value;
            }
            var columnMappings = mappings?.AsList() ?? GetDefaultMappingsForDataReader(connection, tableName, reader, transaction, excludeField).AsList();
            var dbFields = mappings != null ? DbFieldCache.Get(connection, tableName, transaction) : null;
            var sourceOrdinals = new int[columnMappings.Count];
            for (var i = 0; i < columnMappings.Count; i++)
            {
                var sourceOrdinal = reader.GetOrdinal(columnMappings[i].SourceColumn);
                if (dbFields != null)
                {
                    var destinationField = dbFields.GetByUnquotedName(columnMappings[i].DestinationColumn.AsUnquoted(true, dbSetting));
                    var sourceType = reader.GetFieldType(sourceOrdinal);
                    if (destinationField?.Type != null && sourceType != null && !AreMappingTypesCompatible(sourceType, destinationField.Type))
                    {
                        throw new InvalidTypeException($"The type of the source column '{columnMappings[i].SourceColumn}' ({sourceType}) does not match the type of the destination column '{columnMappings[i].DestinationColumn}' ({destinationField.Type}).");
                    }
                }
                sourceOrdinals[i] = sourceOrdinal;
                bulkCopy.ColumnMappings.Add(new MariaDbBulkCopyColumnMapping(i, columnMappings[i].DestinationColumn));
            }
            var filteredReader = new ColumnFilteredDataReader(reader, sourceOrdinals);

            return (bulkCopy, filteredReader);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="transaction"></param>
        /// <param name="excludeField"></param>
        /// <returns></returns>
        private static IEnumerable<MariaDbBulkInsertMapItem> GetDefaultMappingsForDataReader(MySqlConnection connection,
            string tableName,
            IDataReader reader,
            MySqlTransaction transaction,
            Field excludeField = null)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var dbSetting = connection.GetDbSetting();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var dbField = dbFields.GetByUnquotedName(columnName.AsUnquoted(true, dbSetting));
                if (dbField != null && !string.Equals(dbField.Name, excludeField?.Name, System.StringComparison.OrdinalIgnoreCase))
                {
                    yield return new MariaDbBulkInsertMapItem(columnName, dbField.Name);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dbFields"></param>
        /// <param name="qualifiers"></param>
        /// <returns></returns>
        /// <exception cref="PrimaryFieldNotFoundException"></exception>
        private static IEnumerable<Field> GetQualifierFields(string tableName,
            DbFieldCollection dbFields,
            IEnumerable<Field> qualifiers = null)
        {
            if (qualifiers?.Any() == true)
            {
                return qualifiers;
            }

            var primaryOrIdentity = dbFields?.GetPrimary() ?? dbFields?.GetIdentity();

            if (primaryOrIdentity == null)
            {
                throw new PrimaryFieldNotFoundException(
                    $"No primary or identity key found for table '{tableName}'. Provide explicit 'qualifiers' instead.");
            }

            return new[] { primaryOrIdentity.AsField() };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static DbCommand CreateTraceCommand(MySqlConnection connection,
            string commandText,
            int? commandTimeout = null,
            MySqlTransaction transaction = null) =>
            (DbCommand)connection.CreateCommand(commandText, CommandType.Text, commandTimeout, transaction);

        #endregion
    }
}
