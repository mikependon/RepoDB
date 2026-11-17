using System.Data;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Propagates present column values through neighboring gaps.
/// </summary>
internal static class DirectionalFillHelper
{
    #region Public Methods

    /// <summary>
    /// Computes a directional fill before updating the target column.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The target column.</param>
    /// <param name="forward">Whether to scan from the first current row.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The filled table.</returns>
    public static DataTable Fill(DataTable table,
        string column,
        bool forward,
        bool inPlace)
    {
        var source = InspectionHelper.Column(table,
            column);
        var values = InspectionHelper.Rows(table)
            .Select(row => row[source])
            .ToArray();
        Propagate(values,
            forward);
        return TransformationHelper.SetColumn(table,
            source.ColumnName,
            source.DataType,
            values,
            inPlace);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Fills gaps while retaining unseeded edge values.
    /// </summary>
    /// <param name="values">The current column values.</param>
    /// <param name="forward">Whether to scan forward.</param>
    private static void Propagate(object?[] values,
        bool forward)
    {
        object? previous = null;
        for (var step = 0; step < values.Length; step++)
        {
            var index = forward ? step : values.Length - step - 1;
            if (InspectionHelper.IsMissing(values[index]))
            {
                values[index] = previous ?? DBNull.Value;
            }
            else
            {
                previous = values[index];
            }
        }
    }

    #endregion
}
