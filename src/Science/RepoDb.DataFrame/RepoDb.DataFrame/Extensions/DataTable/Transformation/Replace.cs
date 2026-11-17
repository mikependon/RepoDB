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
    /// Replaces matching values using CLR equality.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The target column.</param>
    /// <param name="oldValue">The value to match.</param>
    /// <param name="newValue">The replacement value.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The operation result.</returns>
    public static DataTable Replace(this DataTable table,
        string column,
        object? oldValue,
        object? newValue,
        bool inPlace = false)
    {
        var source = InspectionHelper.Column(table,
            column);
        var oldKey = InspectionHelper.IsMissing(oldValue) ? null : oldValue;
        var values = InspectionHelper.Rows(table)
            .Select(row => Equals(InspectionHelper.IsMissing(row[source]) ? null : row[source],
                oldKey) ? newValue : row[source])
                .ToArray();
        return TransformationHelper.SetColumn(table,
            source.ColumnName,
            source.DataType,
            values,
            inPlace);
    }

    #endregion
}
