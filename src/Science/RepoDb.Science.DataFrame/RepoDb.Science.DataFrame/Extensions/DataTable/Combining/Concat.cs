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
    /// Appends tables with matching ordered names and types.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="others">The additional tables.</param>
    /// <returns>The operation result.</returns>
    public static DataTable Concat(this DataTable table,
        params DataTable[] others)
    {
        ArgumentNullException.ThrowIfNull(others);
        var result = TableHelper.Target(table,
            false);
        foreach (var other in others)
        {
            CombiningHelper.Append(result,
                other);
        }
        return result;
    }

    #endregion
}
