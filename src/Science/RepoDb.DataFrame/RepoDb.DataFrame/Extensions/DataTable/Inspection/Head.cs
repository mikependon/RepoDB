using System.Data;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// Inspects tables without modifying their contents.
/// </summary>
public static partial class DataTableExtensions
{
    #region Public Methods

    /// <summary>
    /// Copies the first current rows.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="count">The nonnegative row limit.</param>
    /// <returns>An independent table.</returns>
    public static DataTable Head(this DataTable table,
        int count = 5)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        return InspectionHelper.CopyRows(table,
            InspectionHelper.Rows(table)
                .Take(count));
    }

    #endregion
}
