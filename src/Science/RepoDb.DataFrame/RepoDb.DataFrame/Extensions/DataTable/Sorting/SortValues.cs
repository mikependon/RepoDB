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
    /// Stably sorts rows by one column.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The sort column.</param>
    /// <param name="ascending">Whether to sort ascending.</param>
    /// <param name="nullsLast">Whether missing values sort last.</param>
    /// <returns>The operation result.</returns>
    public static DataTable SortValues(this DataTable table,
        string column,
        bool ascending = true,
        bool nullsLast = true)
    {
        return SortValues(table,
            new[] { column },
            new[] { ascending },
            nullsLast);
    }

    /// <summary>
    /// Stably sorts rows by multiple columns.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="columns">The ordered sort names.</param>
    /// <param name="ascending">The directions, or null for ascending.</param>
    /// <param name="nullsLast">Whether missing values sort last.</param>
    /// <returns>The sorted table.</returns>
    public static DataTable SortValues(this DataTable table,
        string[] columns,
        bool[]? ascending = null,
        bool nullsLast = true)
    {
        var selected = TableHelper.Columns(table,
            columns);
        var directions = ascending ?? Enumerable.Repeat(true,
            selected.Length)
                .ToArray();
        if (selected.Length == 0 || selected.Length != directions.Length)
        {
            throw new ArgumentException("Sort columns require one direction each.",
                nameof(columns));
        }
        var comparer = new RowComparer(selected,
            directions,
            nullsLast);
        return TableHelper.Project(table,
            table.Columns
                .Cast<DataColumn>(),
            InspectionHelper.Rows(table)
                .OrderBy(row => row,
                    comparer));
    }

    #endregion
}
