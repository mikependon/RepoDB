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
    /// Gets the current row and column counts.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <returns>The table dimensions.</returns>
    public static (int Rows, int Columns) Shape(this DataTable table)
    {
        return (
            InspectionHelper.Rows(table).Count(),
            table.Columns.Count
        );
    }

    #endregion
}
