using System.Data;

namespace RepoDb.Science;

/// <summary>
/// Inspects tables without modifying their contents.
/// </summary>
public static partial class DataTableExtensions
{
    #region Public Methods

    /// <summary>
    /// Gets distinct values in first-seen order.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The column name.</param>
    /// <param name="includeNulls">Whether to include one null value.</param>
    /// <returns>The distinct values.</returns>
    public static IReadOnlyList<object?> Unique(this DataTable table,
        string column,
        bool includeNulls = true)
    {
        return ValueCounts(table,
            column,
            includeNulls)
                .Select(item => item.Value)
            .ToList()
            .AsReadOnly();
    }

    #endregion
}
