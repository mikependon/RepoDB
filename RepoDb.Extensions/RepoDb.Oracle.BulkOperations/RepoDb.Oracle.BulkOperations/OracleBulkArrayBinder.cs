using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Oracle.BulkOperations;

internal class OracleBulkArrayBinder
{
    private const int MaxBindableParametersCount = 65_535;
    private readonly OracleConnection connection;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    public OracleBulkArrayBinder(OracleConnection connection)
    {
        this.connection = connection;
    }

    #region Helpers

    /// <summary>
    /// 
    /// </summary>
    /// <param name="batchSize"></param>
    /// <param name="columnCount"></param>
    /// <returns></returns>
    private int GetBatchSize(
        int? batchSize,
        int columnCount)
    {
        if (batchSize == null)
        {
            batchSize = Math.Min(1000, MaxBindableParametersCount / columnCount);
        }
        return batchSize.Value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private DataTable ToDataTable(
        DbDataReader reader)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataTable"></param>
    /// <param name="columnName"></param>
    /// <param name="arrayBindCount"></param>
    /// <returns></returns>
    private object?[] GetBoundValues(
        DataTable dataTable,
        string columnName,
        int arrayBindCount)
    {
        var values = new object?[arrayBindCount];
        for (var rowIndex = 0; rowIndex < arrayBindCount; rowIndex++)
        {
            values[rowIndex] = dataTable.Rows[rowIndex][columnName] ?? DBNull.Value;
        }
        return values;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataType"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private OracleDbType ToOracleDbType(
        Type dataType)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="columns"></param>
    /// <returns></returns>
    private string BuildInsertStatement(
        string tableName,
        IReadOnlyList<string> columns)
    {
        var columnList = string.Join(", ", columns);
        var parameterList = string.Join(", ", columns.Select(column => ":" + column));
        return $"INSERT INTO {tableName} ({columnList}) VALUES ({parameterList});";
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="reader"></param>
    /// <param name="mappings"></param>
    /// <param name="bulkCopyTimeout"></param>
    /// <param name="batchSize"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> WriteToServerAsync(
        string tableName,
        DbDataReader reader,
        IEnumerable<OracleBulkInsertMapItem> mappings,
        int? bulkCopyTimeout,
        int? batchSize = null,
        OracleTransaction transaction = null,
        CancellationToken cancellationToken = default)
    {
        if (reader.FieldCount == 0)
        {
            return 0;
        }
        return await WriteToServerAsync(tableName,
            ToDataTable(reader),
            mappings,
            bulkCopyTimeout,
            batchSize,
            transaction,
            cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="dataTable"></param>
    /// <param name="mappings"></param>
    /// <param name="bulkCopyTimeout"></param>
    /// <param name="batchSize"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> WriteToServerAsync(
        string tableName,
        DataTable dataTable,
        IEnumerable<OracleBulkInsertMapItem> mappings,
        int? bulkCopyTimeout,
        int? batchSize = null,
        OracleTransaction transaction = null,
        CancellationToken cancellationToken = default)
    {

        if (dataTable.Columns.Count == 0)
        {
            return 0;
        }

        batchSize = GetBatchSize(batchSize, dataTable.Columns.Count);
        var columnNames = dataTable.Columns.OfType<DataColumn>().Select(c => c.ColumnName).ToArray();
        var affectedRows = 0;

        for (var offset = 0; offset < dataTable.Columns.Count; offset += GetBatchSize(batchSize, dataTable.Columns.Count))
        {
            var count = Math.Min(batchSize.Value, (dataTable.Rows.Count - offset));

            await using var command = connection.CreateCommand();

            command.Transaction = transaction;
            command.BindByName = true;
            command.ArrayBindCount = count;
            command.CommandText = BuildInsertStatement(tableName, columnNames);

            foreach (var columnName in columnNames)
            {
                var parameter = new OracleParameter
                {
                    ParameterName = columnName,
                    OracleDbType = ToOracleDbType(dataTable.Columns[columnName].DataType),
                    Direction = ParameterDirection.Input,
                    Value = GetBoundValues(dataTable, columnName, count)
                };

                command.Parameters.Add(parameter);
            }

            affectedRows += await command.ExecuteNonQueryAsync(
                cancellationToken);
        }

        return affectedRows;
    }
}
