using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.SqlServer.BulkOperations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for SqlConnection object.
    /// </summary>
    public static partial class SqlConnectionExtension
    {
        #region Privates

        private static FieldInfo m_systemDataSqlBulkCopyRowsCopiedField = null;
        private static FieldInfo m_microsoftDataSqlBulkCopyRowsCopiedField = null;

        #endregion

        #region System.Data

        /// <summary>
        /// Gets the <see cref="SqlBulkCopy"/> private variable reflected field.
        /// </summary>
        /// <returns>The actual field.</returns>
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

        #region Microsoft.Data

        /// <summary>
        /// Gets the <see cref="SqlBulkCopy"/> private variable reflected field.
        /// </summary>
        /// <returns>The actual field.</returns>
        private static FieldInfo GetRowsCopiedFieldFromMicrosoftDataSqlBulkCopy()
        {
            // Check if the call has made earlier
            if (m_microsoftDataBulkInsertRowsCopiedFieldHasBeenSet == true)
            {
                return m_microsoftDataSqlBulkCopyRowsCopiedField;
            }

            // Set the flag
            m_microsoftDataBulkInsertRowsCopiedFieldHasBeenSet = true;

            // Get the field (whether null or not)
            m_microsoftDataSqlBulkCopyRowsCopiedField = typeof(Microsoft.Data.SqlClient.SqlBulkCopy)
                .GetField("_rowsCopied", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            // Return the value
            return m_microsoftDataSqlBulkCopyRowsCopiedField;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Set the data entities identities.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="entities">The list of the data entities.</param>
        /// <param name="identitiesResult">The result of the bulk operation.</param>
        private static void SetIdentities<TEntity>(IEnumerable<TEntity> entities,
            BulkOperationIdentitiesResult identitiesResult)
            where TEntity : class
        {
            // TODO: Compile this by using the FunctionCache.GetDataEntityPropertyValueSetterFunction<TEntity>(identity);

            // Check if there are entities
            if (entities?.Any() != true)
            {
                return;
            }

            // Get the results
            var results = identitiesResult.Identities?.OfType<object>();
            if (results == null)
            {
                throw new NullReferenceException("No identities returned.");
            }

            // Get the property
            var property = PropertyCache.Get<TEntity>(identitiesResult.IdentityPropertyName);
            if (property == null)
            {
                throw new PropertyNotFoundException($"Identity property not found for type '{typeof(TEntity).FullName}'.");
            }

            // Check the equality
            if (entities.Count() != results.Count())
            {
                throw new InvalidOperationException("The returned identities does not matched the number of the data entities.");
            }

            // Set the identity
            for (var i = 0; i < entities.Count(); i++)
            {
                var entity = entities.ElementAt(i);
                property.PropertyInfo.SetValue(entity, results?.ElementAt(i));
            }
        }

        /// <summary>
        /// Gets the actual name of the table from the database.
        /// </summary>
        /// <param name="tableName">The passed table name.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The actual table name.</returns>
        private static string GetTableName(string tableName,
            IDbSetting dbSetting)
        {
            // Get the schema and table name
            if (tableName.IndexOf(dbSetting.SchemaSeparator) > 0)
            {
                var splitted = tableName.Split(dbSetting.SchemaSeparator.ToCharArray());
                return splitted[1].AsUnquoted(true, dbSetting);
            }

            // Return the unquoted
            return tableName.AsUnquoted(true, dbSetting);
        }

        /// <summary>
        /// Validates whether the transaction object connection is object is equals to the connection object.
        /// </summary>
        /// <param name="connection">The connection object to be validated.</param>
        /// <param name="transaction">The transaction object to compare.</param>
        private static void ValidateTransactionConnectionObject(this IDbConnection connection,
            IDbTransaction transaction)
        {
            if (transaction != null && transaction.Connection != connection)
            {
                throw new InvalidOperationException("The transaction connection object is different from the current connection object.");
            }
        }

        /// <summary>
        /// Returns all the <see cref="DataTable"/> objects of the <see cref="DataTable"/>.
        /// </summary>
        /// <param name="dataTable">The instance of <see cref="DataTable"/> where the list of <see cref="DataColumn"/> will be extracted.</param>
        /// <returns>The list of <see cref="DataColumn"/> objects.</returns>
        private static IEnumerable<DataColumn> GetDataColumns(DataTable dataTable)
        {
            foreach (var column in dataTable.Columns.OfType<DataColumn>())
            {
                yield return column;
            }
        }

        /// <summary>
        /// Returns all the <see cref="DataRow"/> objects of the <see cref="DataTable"/> by state.
        /// </summary>
        /// <param name="dataTable">The instance of <see cref="DataTable"/> where the list of <see cref="DataRow"/> will be extracted.</param>
        /// <param name="rowState">The state of the <see cref="DataRow"/> objects to be extracted.</param>
        /// <returns>The list of <see cref="DataRow"/> objects.</returns>
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
        /// Gets the equivalent list of <see cref="BulkInsertMapItem"/> from the list of the <see cref="Field"/> objects.
        /// </summary>
        /// <param name="fields">The list of <see cref="Field"/> objects to extract.</param>
        /// <returns>The list of <see cref="BulkInsertMapItem"/> objects.</returns>
        private static IEnumerable<BulkInsertMapItem> GetBulkInsertMapItemsFromFields(IEnumerable<Field> fields)
        {
            foreach (var field in fields)
            {
                yield return new BulkInsertMapItem(field.Name, field.Name);
            }
        }

        #endregion

        #region SQL Helpers

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

        private static string GetDropTemporaryTableSqlText(string tempTableName,
            IDbSetting dbSetting)
        {
            return $"DROP TABLE {tempTableName.AsQuoted(dbSetting)};";
        }

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
                throw new MissingFieldException("There is no field(s) defined.");
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
                throw new MissingFieldException("There is no field(s) defined.");
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
