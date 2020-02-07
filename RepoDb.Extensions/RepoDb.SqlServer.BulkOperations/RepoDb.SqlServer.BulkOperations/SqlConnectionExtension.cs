using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for SqlConnection object.
    /// </summary>
    public static partial class SqlConnectionExtension
    {
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
        private static IEnumerable<DataRow> GetDataRows(DataTable dataTable, DataRowState rowState)
        {
            foreach (var row in dataTable.Rows.OfType<DataRow>().Where(r => r.RowState == rowState))
            {
                yield return row;
            }
        }
    }
}
