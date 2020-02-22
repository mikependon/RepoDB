using RepoDb.Extensions;
using RepoDb.Interfaces;
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

        #endregion

        #region SQL Helpers

        private static string GetCreateTemporaryTableSqlText(string tableName,
            string tempTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var builder = new QueryBuilder();
            var selectFields = new List<Field>(qualifiers);

            // Add the fields
            selectFields.AddRange(fields);

            // Compose the statement
            builder.Select()
                .FieldsFrom(selectFields, dbSetting)
                .Into()
                .WriteText(tempTableName.AsQuoted(dbSetting))
                .From()
                .TableNameFrom(tableName, dbSetting)
                .Where()
                .WriteText("1 = 0")
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

        private static string GetBulkUpdateSqlText(string tableName,
            string tempTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
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
            var qualifierFields = qualifiers
                .Select(f => $"S.{f.Name.AsField(dbSetting)} = T.{f.Name.AsField(dbSetting)}")
                .Join(", ");
            var setFields = fields
                .Select(f => $"T.{f.Name.AsField(dbSetting)} = S.{f.Name.AsField(dbSetting)}")
                .Join(", ");
            var builder = new QueryBuilder();

            // Compose the statement
            builder
                .Update()
                .WriteText("T")
                .Set()
                .WriteText(setFields)
                .From()
                .TableNameFrom(tableName, dbSetting)
                .WriteText("T")
                .WriteText("INNER JOIN")
                .TableNameFrom(tempTableName, dbSetting)
                .WriteText("S")
                .WriteText("ON")
                .WriteText(qualifierFields)
                .End();

            // Return the sql
            return builder.ToString();
        }

        private static string GetBulkMergeSqlText(string tableName,
            string tempTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
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
            var qualifierFields = qualifiers
                .Select(f => $"S.{f.Name.AsField(dbSetting)} = T.{f.Name.AsField(dbSetting)}")
                .Join(", ");
            var setFields = fields
                .Select(f => $"T.{f.Name.AsField(dbSetting)} = S.{f.Name.AsField(dbSetting)}")
                .Join(", ");
            var builder = new QueryBuilder();

            // Compose the statement
            builder
                .Update()
                .WriteText("T")
                .Set()
                .WriteText(setFields)
                .From()
                .TableNameFrom(tableName, dbSetting)
                .WriteText("T")
                .WriteText("INNER JOIN")
                .TableNameFrom(tempTableName, dbSetting)
                .WriteText("S")
                .WriteText("ON")
                .WriteText(qualifierFields)
                .End();

            // Return the sql
            return builder.ToString();
        }

        #endregion
    }
}
