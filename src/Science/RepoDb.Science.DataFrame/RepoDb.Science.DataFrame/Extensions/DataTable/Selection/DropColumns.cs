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
    /// Removes selected columns from a copy.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="columns">The column names to remove.</param>
    /// <returns>The operation result.</returns>
    public static DataTable DropColumns(this DataTable table,
        params string[] columns)
    {
        var removed = TableHelper.Columns(table,
            columns)
                .ToHashSet();
        return TableHelper.Project(table,
            table.Columns
                .Cast<DataColumn>()
                .Where(column => !removed.Contains(column)),
            InspectionHelper.Rows(table));
    }

    /// <summary>
    /// Removes selected columns with optional source mutation.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="columns">The column names to remove.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The reduced table.</returns>
    public static DataTable DropColumns(this DataTable table,
        string[] columns,
        bool inPlace)
    {
        if (!inPlace)
        {
            return DropColumns(table,
                columns);
        }
        var selected = TableHelper.Columns(table,
            columns);
        if (selected
            .Any(column => !table.Columns.CanRemove(column)))
        {
            throw new InvalidOperationException("A selected column has constraints or dependencies.");
        }
        foreach (var column in selected)
        {
            table.Columns.Remove(column);
        }
        return table;
    }

    #endregion
}
