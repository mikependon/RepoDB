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
    /// Identifies missing values in current row order.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The target column.</param>
    /// <returns>The operation result.</returns>
    public static bool[] IsNull(this DataTable table,
        string column)
    {
        var source = InspectionHelper.Column(table,
            column);
        return InspectionHelper.Rows(table)
            .Select(row => InspectionHelper.IsMissing(row[source]))
            .ToArray();
    }

    #endregion
}
