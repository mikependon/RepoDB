using System.Data.Common;

namespace RepoDb.Science.Services;

/// <summary>
/// Reads a result set into a table.
/// </summary>
internal static class NotebookTableReader
{
    #region Public Methods

    /// <summary>
    /// Reads the current result set up to the row limit.
    /// </summary>
    /// <param name="reader">The data reader.</param>
    /// <param name="rowLimit">The most rows kept.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The table.</returns>
    public static async Task<NotebookTable> ReadAsync(DbDataReader reader,
        int rowLimit,
        CancellationToken cancellationToken)
    {
        var columns = ColumnsOf(reader);
        var rows = new List<IReadOnlyList<string?>>();
        var isTruncated = false;
        while (!isTruncated && await reader.ReadAsync(cancellationToken)
            .ConfigureAwait(false))
        {
            isTruncated = rows.Count == rowLimit;
            if (!isTruncated)
            {
                rows.Add(RowOf(reader));
            }
        }
        return new NotebookTable(columns,
            rows,
            isTruncated);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Reads the columns.
    /// </summary>
    /// <param name="reader">The data reader.</param>
    /// <returns>The columns.</returns>
    private static List<NotebookColumn> ColumnsOf(DbDataReader reader) => Enumerable
        .Range(0,
            reader.FieldCount)
        .Select(index => new NotebookColumn(reader.GetName(index),
            NotebookValueConverter.TypeNameOf(reader.GetFieldType(index))))
        .ToList();

    /// <summary>
    /// Reads the current row as text.
    /// </summary>
    /// <param name="reader">The data reader.</param>
    /// <returns>The values.</returns>
    private static List<string?> RowOf(DbDataReader reader) => Enumerable
        .Range(0,
            reader.FieldCount)
        .Select(index => NotebookValueConverter.ToText(reader.GetValue(index)))
        .ToList();

    #endregion
}
