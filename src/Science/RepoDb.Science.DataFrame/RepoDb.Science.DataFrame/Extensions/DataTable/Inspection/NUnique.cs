using System.Data;

namespace RepoDb.Science;

/// <summary>
/// Inspects tables without modifying their contents.
/// </summary>
public static partial class DataTableExtensions
{
    #region Public Methods

    /// <summary>
    /// Counts distinct values.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The column name.</param>
    /// <param name="includeNulls">Whether to count missing values.</param>
    /// <returns>The distinct value count.</returns>
    public static int NUnique(this DataTable table,
        string column,
        bool includeNulls = false)
    {
        return ValueCounts(table,
            column,
            includeNulls).Count;
    }

    #endregion
}
