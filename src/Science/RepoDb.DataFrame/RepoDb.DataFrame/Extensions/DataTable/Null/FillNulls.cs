using System.Data;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// Provides analytical DataTable operations.
/// </summary>
public static partial class DataTableExtensions
{
    #region Public Methods

    /// <summary>
    /// Replaces missing values in one column.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The target column.</param>
    /// <param name="value">The replacement value.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The operation result.</returns>
    public static DataTable FillNulls(this DataTable table,
        string column,
        object? value,
        bool inPlace = false)
    {
        var source = InspectionHelper.Column(table,
            column);
        var values = InspectionHelper.Rows(table)
            .Select(row => InspectionHelper.IsMissing(row[source]) ? value : row[source])
            .ToArray();
        return TransformationHelper.SetColumn(table,
            source.ColumnName,
            source.DataType,
            values,
            inPlace);
    }

    #endregion
}
