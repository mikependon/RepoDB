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
    /// Counts values in first-seen order using CLR equality.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The column name.</param>
    /// <param name="includeNulls">Whether to include missing values.</param>
    /// <returns>The value frequencies.</returns>
    public static IReadOnlyList<ValueCount> ValueCounts(this DataTable table,
        string column,
        bool includeNulls = false)
    {
        var dataColumn = InspectionHelper.Column(table,
            column);
        return InspectionHelper.Rows(table)
            .Select(row => row[dataColumn])
            .Where(value => includeNulls || !InspectionHelper.IsMissing(value))
            .GroupBy(value => InspectionHelper.IsMissing(value) ? null : value)
            .Select(group => new ValueCount(group.Key,
                group
                    .Count()))
            .ToList()
            .AsReadOnly();
    }

    #endregion
}
