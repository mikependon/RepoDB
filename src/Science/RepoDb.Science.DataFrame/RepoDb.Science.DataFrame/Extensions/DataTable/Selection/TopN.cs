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
    /// Selects rows with the highest present values.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The comparable column.</param>
    /// <param name="count">The nonnegative row limit.</param>
    /// <returns>The operation result.</returns>
    public static DataTable TopN(this DataTable table,
        string column,
        int count = 5)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        var source = InspectionHelper.Column(table,
            column);
        return table
            .Filter(row => !InspectionHelper.IsMissing(row[source]))
            .SortValues(source.ColumnName,
                false)
            .Head(count);
    }

    #endregion
}
