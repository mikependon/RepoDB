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
            IEnumerable<BulkInsertMapItem> mappings)
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
        private static IEnumerable<BulkInsertMapItem> AddOrderColumnMapping(IEnumerable<BulkInsertMapItem> mappings)
        {
            var list = mappings.AsList();
            list.Add(new BulkInsertMapItem("__RepoDb_OrderColumn", "__RepoDb_OrderColumn"));
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        private static IEnumerable<BulkInsertMapItem> GetBulkInsertMapItemsFromFields(IEnumerable<Field> fields)
        {
            foreach (var field in fields)
            {
                yield return new BulkInsertMapItem(field.Name, field.Name);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        internal static void ThrowIfNullOrEmpty(DbDataReader reader)
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
        /// <param name="tableName"></param>
        /// <param name="tempTableName"></param>
        /// <param name="fields"></param>
        /// <param name="dbSetting"></param>
        /// <param name="isReturnIdentity"></param>
        /// <returns></returns>
        private static string GetCreateTemporaryTableSqlText(string tableName,
            string tempTableName,
            IEnumerable<Field> fields,
            IDbSetting dbSetting,
            bool isReturnIdentity)
        {
            var builder = new QueryBuilder();

            // Compose the statement
            builder
                .Clear()
                .Select()
                .FieldsFrom(fields, dbSetting);

            // Return Identity
            if (isReturnIdentity)
            {
                builder.WriteText(", CONVERT(INT, NULL) AS [__RepoDb_OrderColumn]");
            };

            // Continuation
            builder
                .Into()
                .WriteText(tempTableName.AsQuoted(dbSetting))
                .From()
                .TableNameFrom(tableName, dbSetting)
                .Where()
                .WriteText("(1 = 0)")
                .End();

            // Return the text
            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tempTableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetCreateTemporaryTableClusteredIndexSqlText(string tempTableName,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            // Validate the presence
            if (qualifiers?.Any() != true)
            {
                throw new MissingFieldException("There is no qualifier field(s) defined.");
            }

            // Variables needed
            var clusteredIndexFields = qualifiers
                .Select(f => $"{f.Name.AsQuoted(dbSetting)} ASC")
                .Join(", ");
            var builder = new QueryBuilder();

            // Compose the statement
            builder
                .Clear()
                .WriteText("CREATE CLUSTERED INDEX")
                .WriteText($"IX_{tempTableName}".AsQuoted(dbSetting))
                .On()
                .WriteText(tempTableName.AsQuoted(dbSetting))
                .OpenParen()
                .WriteText(clusteredIndexFields)
                .CloseParen()
                .End();

            // Return the sql
            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tempTableName"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetDropTemporaryTableSqlText(string tempTableName,
            IDbSetting dbSetting) =>
            $"DROP TABLE {tempTableName.AsQuoted(dbSetting)};";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tempTableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="hints"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetBulkDeleteSqlText(string tableName,
            string tempTableName,
            IEnumerable<Field> qualifiers,
            string hints,
            IDbSetting dbSetting)
        {
            // Validate the presence
            if (qualifiers?.Any() != true)
            {
                throw new MissingFieldException("There is no qualifier field(s) defined.");
            }

            // Variables needed
            var builder = new QueryBuilder();

            // Compose the statement
            builder
                .Clear()
                .Delete()
                .WriteText("T")
                .From()
                .TableNameFrom(tableName, dbSetting)
                .WriteText("T")
                .HintsFrom(hints)
                .WriteText("INNER JOIN")
                .TableNameFrom(tempTableName, dbSetting)
                .WriteText("S")
                .WriteText("ON")
                .WriteText(qualifiers
                    .Select(
                        field => field.AsJoinQualifier("S", "T", true, dbSetting))
                            .Join(" AND "))
                .End();

            // Return the sql
            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tempTableName"></param>
        /// <param name="fields"></param>
        /// <param name="identityField"></param>
        /// <param name="hints"></param>
        /// <param name="dbSetting"></param>
        /// <param name="isReturnIdentity"></param>
        /// <param name="forceIdentityColumn"></param>
        /// <returns></returns>
        private static string GetBulkInsertSqlText(string tableName,
            string tempTableName,
            IEnumerable<Field> fields,
            Field identityField,
            string hints,
            IDbSetting dbSetting,
            bool isReturnIdentity,
            bool forceIdentityColumn)
        {
            // Validate the presence
            if (fields?.Any() != true)
            {
                throw new MissingFieldException("There are no field(s) defined.");
            }

            // Variables needed
            var builder = new QueryBuilder();

            // Insertable fields
            var insertableFields = fields
                .Where(field => forceIdentityColumn == true || string.Equals(field.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase) == false);

            // Compose the statement
            builder.Clear();

            // SET IDENTITY_INSERT ON
            if (forceIdentityColumn)
            {
                builder
                    .WriteText("SET IDENTITY_INSERT")
                    .TableNameFrom(tableName, dbSetting)
                    .WriteText("ON;");
            }

            builder
                // MERGE T USING S
                .Merge()
                .TableNameFrom(tableName, dbSetting)
                .HintsFrom(hints)
                .As("T")
                .Using()
                .OpenParen()
                .Select()
                .Top()
                .WriteText("100 PERCENT")
                //.FieldsFrom(fields, dbSetting)
                .WriteText("*") // Including the [__RepoDb_OrderColumn]
                .From()
                .TableNameFrom(tempTableName, dbSetting);

            // Return Identity
            if (isReturnIdentity && identityField != null)
            {
                builder
                    .OrderBy()
                    .WriteText("[__RepoDb_OrderColumn]")
                    .Ascending();
            }

            // Continuation
            builder
                .CloseParen()
                .As("S")
                // QUALIFIERS
                .On()
                .OpenParen()
                .WriteText("1 = 0")
                .CloseParen()
                // WHEN NOT MATCHED THEN INSERT VALUES
                .When()
                .Not()
                .Matched()
                .Then()
                .Insert()
                .OpenParen()
                .FieldsFrom(insertableFields, dbSetting)
                .CloseParen()
                .Values()
                .OpenParen()
                .AsAliasFieldsFrom(insertableFields, "S", dbSetting)
                .CloseParen();

            // Set the output
            if (isReturnIdentity == true && identityField != null)
            {
                builder
                    .WriteText(string.Concat("OUTPUT INSERTED.", identityField.Name.AsField(dbSetting)))
                        .As("[Result],")
                    .WriteText("S.[__RepoDb_OrderColumn]")
                        .As("[OrderColumn]");
            }

            // End
            builder.End();

            // SET IDENTITY_INSERT OFF (probably not necessary, but it won't hurt)
            if (forceIdentityColumn)
            {
                builder
                    .WriteText("SET IDENTITY_INSERT")
                    .TableNameFrom(tableName, dbSetting)
                    .WriteText("OFF;");
            }

            // Return the sql
            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tempTableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="primaryField"></param>
        /// <param name="identityField"></param>
        /// <param name="hints"></param>
        /// <param name="dbSetting"></param>
        /// <param name="isReturnIdentity"></param>
        /// <param name="forceIdentityColumn"></param>
        /// <returns></returns>
        private static string GetBulkMergeSqlText(string tableName,
            string tempTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field primaryField,
            Field identityField,
            string hints,
            IDbSetting dbSetting,
            bool isReturnIdentity,
            bool forceIdentityColumn)
        {
            // Validate the presence
            if (fields?.Any() != true)
            {
                throw new MissingFieldException("There are no field(s) defined.");
            }

            if (qualifiers?.Any() != true)
            {
                throw new MissingFieldException("There is no qualifier field(s) defined.");
            }

            // Variables needed
            var builder = new QueryBuilder();

            // Insertable fields
            var insertableFields = fields
                .Where(field => forceIdentityColumn == true || string.Equals(field.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase) == false);

            // Updatable fields
            var updateableFields = fields
                .Where(field => field != identityField && field != primaryField)
                .Where(field =>
                    qualifiers.Any(
                        q => string.Equals(q.Name, field.Name, StringComparison.OrdinalIgnoreCase)) == false);

            // Compose the statement
            builder.Clear();

            // SET IDENTITY_INSERT ON
            if (forceIdentityColumn)
            {
                builder
                    .WriteText("SET IDENTITY_INSERT")
                    .TableNameFrom(tableName, dbSetting)
                    .WriteText("ON;");
            }

            builder
                // MERGE T USING S
                .Merge()
                .TableNameFrom(tableName, dbSetting)
                .HintsFrom(hints)
                .As("T")
                .Using()
                .OpenParen()
                .Select()
                .Top()
                .WriteText("100 PERCENT")
                //.FieldsFrom(fields, dbSetting)
                .WriteText("*") // Including the [__RepoDb_OrderColumn]
                .From()
                .TableNameFrom(tempTableName, dbSetting);

            // Return Identity
            if (isReturnIdentity && identityField != null)
            {
                builder
                    .OrderBy()
                    .WriteText("[__RepoDb_OrderColumn]")
                    .Ascending();
            }

            // Continuation
            builder
                .CloseParen()
                .As("S")
                // QUALIFIERS
                .On()
                .OpenParen()
                .WriteText(qualifiers
                    .Select(
                        field => field.AsJoinQualifier("S", "T", true, dbSetting))
                            .Join(" AND "))
                .CloseParen();

            if (insertableFields?.Any() == true)
            {
                // WHEN NOT MATCHED THEN INSERT VALUES
                builder
                    .When()
                    .Not()
                    .Matched()
                    .Then()
                    .Insert()
                    .OpenParen()
                    .FieldsFrom(insertableFields, dbSetting)
                    .CloseParen()
                    .Values()
                    .OpenParen()
                    .AsAliasFieldsFrom(insertableFields, "S", dbSetting)
                    .CloseParen();
            }

            if (updateableFields?.Any() == true)
            {
                // WHEN MATCHED THEN UPDATE SET
                builder
                    .When()
                    .Matched()
                    .Then()
                    .Update()
                    .Set()
                    .FieldsAndAliasFieldsFrom(updateableFields, "T", "S", dbSetting);
            }

            // Set the output
            if (isReturnIdentity == true && identityField != null)
            {
                builder
                    .WriteText(string.Concat("OUTPUT INSERTED.", identityField.Name.AsField(dbSetting)))
                        .As("[Result],")
                    .WriteText("S.[__RepoDb_OrderColumn]")
                        .As("[OrderColumn]");
            }

            // End the builder
            builder.End();

            // SET IDENTITY_INSERT OFF (probably not necessary, but it won't hurt)
            if (forceIdentityColumn)
            {
                builder
                    .WriteText("SET IDENTITY_INSERT")
                    .TableNameFrom(tableName, dbSetting)
                    .WriteText("OFF;");
            }

            // Return the sql
            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tempTableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="primaryField"></param>
        /// <param name="identityField"></param>
        /// <param name="hints"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetBulkUpdateSqlText(string tableName,
            string tempTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field primaryField,
            Field identityField,
            string hints,
            IDbSetting dbSetting)
        {
            // Validate the presence
            if (fields?.Any() != true)
            {
                throw new MissingFieldException("There are no field(s) defined.");
            }

            if (qualifiers?.Any() != true)
            {
                throw new MissingFieldException("There is no qualifier field(s) defined.");
            }

            // Variables needed
            var builder = new QueryBuilder();

            // Updatable fields
            var updateableFields = fields
                .Where(field => field != identityField && field != primaryField)
                .Where(field =>
                    qualifiers.Any(
                        q => string.Equals(q.Name, field.Name, StringComparison.OrdinalIgnoreCase)) == false);

            // Compose the statement
            builder
                .Clear()
                .Update()
                .WriteText("T")
                .Set()
                .FieldsAndAliasFieldsFrom(updateableFields, "T", "S", dbSetting)
                .From()
                .TableNameFrom(tableName, dbSetting)
                .WriteText("T")
                .HintsFrom(hints)
                .WriteText("INNER JOIN")
                .TableNameFrom(tempTableName, dbSetting)
                .WriteText("S")
                .WriteText("ON")
                .WriteText(qualifiers
                    .Select(
                        field => field.AsJoinQualifier("S", "T", true, dbSetting))
                            .Join(" AND "))
                .End();

            // Return the sql
            return builder.ToString();
        }

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
