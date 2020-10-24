using Microsoft.Data.SqlClient;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.SqlServer.BulkOperations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for SqlConnection object.
    /// </summary>
    public static partial class SqlConnectionExtension
    {
        #region Privates

        private static FieldInfo m_systemDataSqlBulkCopyRowsCopiedField = null;
        private static bool m_systemDataBulkInsertRowsCopiedFieldHasBeenSet = false;

        #endregion

        #region System.Data

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static FieldInfo GetRowsCopiedFieldFromSystemDataSqlBulkCopy()
        {
            // Check if the call has made earlier
            if (m_systemDataBulkInsertRowsCopiedFieldHasBeenSet == true)
            {
                return m_systemDataSqlBulkCopyRowsCopiedField;
            }

            // Set the flag
            m_systemDataBulkInsertRowsCopiedFieldHasBeenSet = true;

            // Get the field (whether null or not)
            m_systemDataSqlBulkCopyRowsCopiedField = typeof(System.Data.SqlClient.SqlBulkCopy)
                .GetField("_rowsCopied", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            // Return the value
            return m_systemDataSqlBulkCopyRowsCopiedField;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <param name="reader"></param>
        /// <param name="identityDbField"></param>
        private static int SetIdentityForEntities<TEntity>(IEnumerable<TEntity> entities,
            DbDataReader reader,
            DbField identityDbField)
            where TEntity : class
        {
            var entityType = entities?.FirstOrDefault()?.GetType() ?? typeof(TEntity);
            var list = entities.AsList();
            var result = 0;

            if (entityType.IsDictionaryStringObject())
            {
                while (reader.Read())
                {
                    var value = Converter.DbNullToNull(reader.GetFieldValue<object>(0));
                    var dictionary = (IDictionary<string, object>)list[result];
                    dictionary[identityDbField.Name] = value;
                    result++;
                }
            }
            else
            {
                var func = Compiler.GetPropertySetterFunc<TEntity>(identityDbField.Name);
                while (reader.Read())
                {
                    var value = Converter.DbNullToNull(reader.GetFieldValue<object>(0));
                    var entity = list[result];
                    func(entity, value);
                    result++;
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

            if (entityType.IsDictionaryStringObject())
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    var value = Converter.DbNullToNull(await reader.GetFieldValueAsync<object>(0, cancellationToken));
                    var dictionary = (IDictionary<string, object>)list[result];
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
                    var entity = list[result];
                    func(entity, value);
                    result++;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSqlBulkCopy"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMappingCollection"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMapping"></typeparam>
        /// <param name="sqlBulkCopy"></param>
        /// <param name="mappings"></param>
        private static void AddMappings<TSqlBulkCopy, TSqlBulkCopyColumnMappingCollection, TSqlBulkCopyColumnMapping>(TSqlBulkCopy sqlBulkCopy,
            IEnumerable<BulkInsertMapItem> mappings)
            where TSqlBulkCopy : class
            where TSqlBulkCopyColumnMappingCollection : class
        {
            var columnMappingsProperty = Compiler.GetPropertyGetterFunc<TSqlBulkCopy, TSqlBulkCopyColumnMappingCollection>("ColumnMappings");
            var columnMappingsInstance = columnMappingsProperty(sqlBulkCopy);
            var types = new[] { typeof(string), typeof(string) };
            var addMethod = Compiler.GetParameterizedMethodFunc<TSqlBulkCopyColumnMappingCollection, TSqlBulkCopyColumnMapping>("Add", types);
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
        /// <param name="fields"></param>
        /// <returns></returns>
        private static IEnumerable<BulkInsertMapItem> GetBulkInsertMapItemsFromFields(IEnumerable<Field> fields)
        {
            foreach (var field in fields)
            {
                yield return new BulkInsertMapItem(field.Name, field.Name);
            }
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
        /// <returns></returns>
        private static string GetCreateTemporaryTableSqlText(string tableName,
            string tempTableName,
            IEnumerable<Field> fields,
            IDbSetting dbSetting)
        {
            var builder = new QueryBuilder();

            // Compose the statement
            builder
                .Clear()
                .Select()
                .FieldsFrom(fields, dbSetting)
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
                throw new MissingFieldException("There is no qualifer field(s) defined.");
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
                throw new MissingFieldException("There is no qualifer field(s) defined.");
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
                        field => field.AsJoinQualifier("S", "T", dbSetting))
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
        /// <returns></returns>
        private static string GetBulkInsertSqlText(string tableName,
            string tempTableName,
            IEnumerable<Field> fields,
            Field identityField,
            string hints,
            IDbSetting dbSetting)
        {
            // Validate the presence
            if (fields?.Any() != true)
            {
                throw new MissingFieldException("There are no field(s) defined.");
            }

            // Variables needed
            var setFields = fields
                .Select(field => field.AsJoinQualifier("T", "S", dbSetting))
                .Join(", ");
            var builder = new QueryBuilder();

            // Insertable fields
            var insertableFields = fields
                .Where(field => string.Equals(field.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase) == false);

            // Compose the statement
            builder.Clear()
                .Insert()
                .Into()
                .TableNameFrom(tableName, dbSetting)
                .HintsFrom(hints)
                .OpenParen()
                .FieldsFrom(insertableFields, dbSetting)
                .CloseParen();

            // Set the output
            if (identityField != null)
            {
                builder
                    .WriteText(string.Concat("OUTPUT INSERTED.", identityField.Name.AsField(dbSetting)))
                    .As("[Result]");
            }

            // Continuation
            builder
                .Select()
                .FieldsFrom(insertableFields, dbSetting)
                .From()
                .TableNameFrom(tempTableName, dbSetting)
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
        /// <param name="qualifiers"></param>
        /// <param name="primaryField"></param>
        /// <param name="identityField"></param>
        /// <param name="hints"></param>
        /// <param name="dbSetting"></param>
        /// <param name="isReturnIdentity"></param>
        /// <returns></returns>
        private static string GetBulkMergeSqlText(string tableName,
            string tempTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field primaryField,
            Field identityField,
            string hints,
            IDbSetting dbSetting,
            bool isReturnIdentity)
        {
            // Validate the presence
            if (fields?.Any() != true)
            {
                throw new MissingFieldException("There are no field(s) defined.");
            }

            if (qualifiers?.Any() != true)
            {
                throw new MissingFieldException("There is no qualifer field(s) defined.");
            }

            // Variables needed
            var setFields = fields
                .Select(field => field.AsJoinQualifier("T", "S", dbSetting))
                .Join(", ");
            var builder = new QueryBuilder();

            // Insertable fields
            var insertableFields = fields
                .Where(field => string.Equals(field.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase) == false);

            // Updatable fields
            var updateableFields = fields
                .Where(field => field != identityField && field != primaryField)
                .Where(field =>
                    qualifiers.Any(
                        q => string.Equals(q.Name, field.Name, StringComparison.OrdinalIgnoreCase)) == false);

            // Compose the statement
            builder.Clear()
                // MERGE T USING S
                .Merge()
                .TableNameFrom(tableName, dbSetting)
                .As("T")
                .HintsFrom(hints)
                .Using()
                .OpenParen()
                .Select()
                .FieldsFrom(fields, dbSetting)
                .From()
                .TableNameFrom(tempTableName, dbSetting)
                .CloseParen()
                .As("S")
                // QUALIFIERS
                .On()
                .OpenParen()
                .WriteText(qualifiers?
                    .Select(
                        field => field.AsJoinQualifier("S", "T", dbSetting))
                            .Join(" AND "))
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
                .CloseParen()
                // WHEN MATCHED THEN UPDATE SET
                .When()
                .Matched()
                .Then()
                .Update()
                .Set()
                .FieldsAndAliasFieldsFrom(updateableFields, "T", "S", dbSetting);

            // Set the output
            if (isReturnIdentity && identityField != null)
            {
                builder
                    .WriteText(string.Concat("OUTPUT INSERTED.", identityField.Name.AsField(dbSetting)))
                    .As("[Result]");
            }

            // End the builder
            builder.End();

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
                throw new MissingFieldException("There is no qualifer field(s) defined.");
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
                        field => field.AsJoinQualifier("S", "T", dbSetting))
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
