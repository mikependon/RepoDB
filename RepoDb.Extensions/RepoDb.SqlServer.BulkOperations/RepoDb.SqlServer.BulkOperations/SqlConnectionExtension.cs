using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.SqlServer.BulkOperations;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for SqlConnection object.
    /// </summary>
    public static partial class SqlConnectionExtension
    {
        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static DbCommand CreateTraceCommand(SqlConnection connection,
            string commandText,
            int? commandTimeout = null,
            SqlTransaction transaction = null) =>
            (DbCommand)connection.CreateCommand(commandText, CommandType.Text, commandTimeout, transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <param name="reader"></param>
        /// <param name="identityField"></param>
        private static int SetIdentityForEntities<TEntity>(IEnumerable<TEntity> entities,
            DbDataReader reader,
            Field identityField)
            where TEntity : class
        {
            var entityType = entities?.FirstOrDefault()?.GetType() ?? typeof(TEntity);
            var list = entities.AsList();
            var result = 0;

            if (TypeCache.Get(entityType).IsDictionaryStringObject())
            {
                while (reader.Read())
                {
                    var value = Converter.DbNullToNull(reader.GetFieldValue<object>(0));
                    var index = reader.GetFieldValue<int>(1);
                    var dictionary = (IDictionary<string, object>)list[index < 0 ? result : index];
                    dictionary[identityField.Name] = value;
                    result++;
                }
            }
            else
            {
                var func = Compiler.GetPropertySetterFunc<TEntity>(identityField.Name);
                if (func != null)
                {
                    while (reader.Read())
                    {
                        var value = Converter.DbNullToNull(reader.GetFieldValue<object>(0));
                        var index = reader.GetFieldValue<int>(1);
                        var entity = list[(index < 0 ? result : index)];
                        func(entity, value);
                        result++;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <param name="reader"></param>
        /// <param name="identityDbField"></param>
        /// <param name="cancellationToken"></param>
        private static async Task<int> SetIdentityForEntitiesAsync<TEntity>(IEnumerable<TEntity> entities,
            DbDataReader reader,
            DbField identityDbField,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entityType = entities?.FirstOrDefault()?.GetType() ?? typeof(TEntity);
            var list = entities.AsList();
            var result = 0;

            if (TypeCache.Get(entityType).IsDictionaryStringObject())
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    var value = Converter.DbNullToNull(await reader.GetFieldValueAsync<object>(0, cancellationToken));
                    var index = await reader.GetFieldValueAsync<int>(1, cancellationToken);
                    var dictionary = (IDictionary<string, object>)list[(index < 0 ? result : index)];
                    dictionary[identityDbField.Name] = value;
                    result++;
                }
            }
            else
            {
                var func = Compiler.GetPropertySetterFunc<TEntity>(identityDbField.Name);
                while (await reader.ReadAsync(cancellationToken))
                {
                    var value = Converter.DbNullToNull(await reader.GetFieldValueAsync<object>(0, cancellationToken));
                    var index = await reader.GetFieldValueAsync<int>(1, cancellationToken);
                    var entity = list[(index < 0 ? result : index)];
                    func(entity, value);
                    result++;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="reader"></param>
        /// <param name="identityColumn"></param>
        /// <returns></returns>
        private static int SetIdentityForEntities(DataTable dataTable,
            DbDataReader reader,
            DataColumn identityColumn)
        {
            var result = 0;
            while (reader.Read())
            {
                var value = Converter.DbNullToNull(reader.GetFieldValue<object>(0));
                dataTable.Rows[result][identityColumn] = value;
                result++;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="reader"></param>
        /// <param name="identityColumn"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> SetIdentityForEntitiesAsync(DataTable dataTable,
            DbDataReader reader,
            DataColumn identityColumn,
            CancellationToken cancellationToken = default)
        {
            var result = 0;
            while (await reader.ReadAsync(cancellationToken))
            {
                var value = Converter.DbNullToNull(await reader.GetFieldValueAsync<object>(0, cancellationToken));
                dataTable.Rows[result][identityColumn] = value;
                result++;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlBulkCopy"></param>
        /// <param name="mappings"></param>
        private static void AddMappings(SqlBulkCopy sqlBulkCopy,
            IEnumerable<SqlServerBulkInsertMapItem> mappings)
        {
            var columnMappingsProperty = Compiler.GetPropertyGetterFunc<SqlBulkCopy, SqlBulkCopyColumnMappingCollection>("ColumnMappings");
            var columnMappingsInstance = columnMappingsProperty(sqlBulkCopy);
            var types = new[] { typeof(string), typeof(string) };
            var addMethod = Compiler.GetParameterizedMethodFunc<SqlBulkCopyColumnMappingCollection, SqlBulkCopyColumnMapping>("Add", types);
            mappings
                .AsList()
                .ForEach(mapItem =>
                    addMethod(columnMappingsInstance, new[] { mapItem.SourceColumn, mapItem.DestinationColumn }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private static IEnumerable<Field> GetDictionaryStringObjectFields(IDictionary<string, object> dictionary)
        {
            foreach (var kvp in dictionary)
            {
                yield return new Field(kvp.Key, kvp.Value?.GetType());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="qualifiers"></param>
        /// <returns></returns>
        private static IEnumerable<Field> ParseExpression<TEntity>(Expression<Func<TEntity, object>> qualifiers)
            where TEntity : class =>
            qualifiers != null ? Field.Parse<TEntity>(qualifiers) : default;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetTableName(string tableName,
            IDbSetting dbSetting) =>
            DataEntityExtension.GetTableName(tableName, dbSetting);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        private static void ValidateTransactionConnectionObject(this IDbConnection connection,
            IDbTransaction transaction)
        {
            if (transaction != null && transaction.Connection != connection)
            {
                throw new InvalidOperationException("The transaction connection object is different from the current connection object.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private static IEnumerable<DataColumn> GetDataColumns(DataTable dataTable)
        {
            foreach (var column in dataTable.Columns.OfType<DataColumn>())
            {
                yield return column;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="rowState"></param>
        /// <returns></returns>
        private static IEnumerable<DataRow> GetDataRows(DataTable dataTable,
            DataRowState? rowState = null)
        {
            var rows = dataTable.Rows.OfType<DataRow>();
            if (rowState.HasValue == true)
            {
                rows = rows.Where(r => r.RowState == rowState);
            }
            foreach (var row in rows)
            {
                yield return row;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private static void AddOrderColumn(DataTable dataTable)
        {
            if (dataTable == null)
            {
                return;
            }
            var column = new DataColumn("__RepoDb_OrderColumn", typeof(int));
            dataTable.Columns.Add(column);
            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                dataTable.Rows[i][column] = i;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mappings"></param>
        private static IEnumerable<SqlServerBulkInsertMapItem> AddOrderColumnMapping(IEnumerable<SqlServerBulkInsertMapItem> mappings)
        {
            var list = mappings.AsList();
            list.Add(new SqlServerBulkInsertMapItem("__RepoDb_OrderColumn", "__RepoDb_OrderColumn"));
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        private static IEnumerable<SqlServerBulkInsertMapItem> GetBulkInsertMapItemsFromFields(IEnumerable<Field> fields)
        {
            foreach (var field in fields)
            {
                yield return new SqlServerBulkInsertMapItem(field.Name, field.Name);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        internal static void ThrowIfNullOrEmpty(
            DbDataReader reader)
        {
            if (reader == null)
            {
                throw new NullReferenceException("The reader must not be null.");
            }
            if (reader.HasRows == false)
            {
                throw new EmptyException("The reader must contain at least a single row.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        internal static void ThrowIfNullOrEmpty(DataTable dataTable)
        {
            if (dataTable == null)
            {
                throw new NullReferenceException("The data table must not be null.");
            }
            if (dataTable.Rows.Count <= 0)
            {
                throw new EmptyException("The data table must contain at least a single row.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        internal static void ThrowIfNullOrEmpty<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class
        {
            if (entities == null)
            {
                throw new NullReferenceException("The entities must not be null.");
            }
            if (entities.Any() == false)
            {
                throw new EmptyException("The entities must not be empty.");
            }
        }

        private static void CommitTransaction(IDbTransaction transaction,
            bool hasTransaction)
        {
            if (hasTransaction == false)
            {
                transaction?.Commit();
            }
        }

        private static void RollbackTransaction(IDbTransaction transaction,
            bool hasTransaction)
        {
            if (hasTransaction == false)
            {
                transaction?.Rollback();
            }
        }

        private static void DisposeTransaction(IDbTransaction transaction,
            bool hasTransaction)
        {
            if (hasTransaction == false)
            {
                transaction?.Dispose();
            }
        }

        private static T CreateOrValidateCurrentTransaction<T>(IDbConnection connection,
            T transaction)
            where T : DbTransaction
        {
            // Check the transaction
            if (transaction == null)
            {
                // Add the transaction if not present
                return (T)connection.EnsureOpen().BeginTransaction();
            }

            // Validate the objects
            ValidateTransactionConnectionObject(connection, transaction);

            return transaction;
        }

        private static async Task<T> CreateOrValidateCurrentTransactionAsync<T>(IDbConnection connection,
            T transaction,
            CancellationToken cancellationToken = default)
            where T : DbTransaction
        {
            // Check the transaction
            if (transaction == null)
            {
                // Add the transaction if not present
                return (T)(await connection.EnsureOpenAsync(cancellationToken)).BeginTransaction();
            }

            // Validate the objects
            ValidateTransactionConnectionObject(connection, transaction);

            return transaction;
        }

        private static string CreateBulkUpdateTempTableName(string tableName,
            bool? usePhysicalPseudoTempTable,
            IDbSetting dbSetting) =>
            CreateBulkTempTableName(tableName, "Update", usePhysicalPseudoTempTable, dbSetting);

        private static string CreateBulkMergeTempTableName(string tableName,
            bool? usePhysicalPseudoTempTable,
            IDbSetting dbSetting) =>
            CreateBulkTempTableName(tableName, "Merge", usePhysicalPseudoTempTable, dbSetting);

        private static string CreateBulkInsertTempTableName(string tableName,
            bool? usePhysicalPseudoTempTable,
            IDbSetting dbSetting) =>
            CreateBulkTempTableName(tableName, "Insert", usePhysicalPseudoTempTable, dbSetting);

        private static string CreateBulkDeleteTempTableName(string tableName,
            bool? usePhysicalPseudoTempTable,
            IDbSetting dbSetting) =>
            CreateBulkTempTableName(tableName, "Delete", usePhysicalPseudoTempTable, dbSetting);

        private static string CreateBulkTempTableName(string tableName,
            string operation,
            bool? usePhysicalPseudoTempTable,
            IDbSetting dbSetting)
        {
            var tempTableName = new StringBuilder();

            // Must be fixed name so the RepoDb.Core caches will not be bloated
            tempTableName
                .Append("_RepoDb_Bulk")
                .Append(operation)
                .Append('_')
                .Append(GetTableName(tableName, dbSetting).AsUnquoted(dbSetting));

            // Add a # prefix if not physical
            if (usePhysicalPseudoTempTable != true)
                tempTableName.Insert(0, '#');

            return tempTableName.ToString();
        }

        #endregion

        #region SQL Helpers

        /// <summary>
        ///
        /// </summary>
        /// <param name="field"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private static DataTable CreateDataTableWithSingleColumn(Field field,
            IEnumerable<object> values)
        {
            // Variables
            var table = new DataTable();
            var column = table
                .Columns
                .Add(field.Name, field.Type);

            // Add the values
            foreach (var value in values)
            {
                var row = table.NewRow();
                row[column] = value;
                table.Rows.Add(row);
            }

            // Commit
            table.AcceptChanges();

            // Return the table
            return table;
        }

        #endregion
    }
}
