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
    /// Returns all most frequent values in first-seen order.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The target column.</param>
    /// <param name="includeNulls">Whether missing values participate.</param>
    /// <returns>The operation result.</returns>
    public static IReadOnlyList<object?> Mode(this DataTable table,
        string column,
        bool includeNulls = false)
    {
        var frequencies = ValueCounts(table,
            column,
            includeNulls);
        var maximum = frequencies.Count == 0 ? 0 : frequencies
            .Max(item => item.Count);
        return frequencies
            .Where(item => item.Count == maximum)
            .Select(item => item.Value)
            .ToList()
            .AsReadOnly();
    }

    #endregion
}
