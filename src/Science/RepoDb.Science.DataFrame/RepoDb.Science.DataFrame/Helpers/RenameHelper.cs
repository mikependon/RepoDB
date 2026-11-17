using System.Data;
using System.Globalization;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Renames validated columns without intermediate name collisions.
/// </summary>
internal static class RenameHelper
{
    #region Public Methods

    /// <summary>
    /// Applies final names through temporary unique names.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="names">The final names.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The renamed table.</returns>
    public static DataTable Execute(DataTable table,
        string[] names,
        bool inPlace)
    {
        var result = TableHelper.Target(table,
            inPlace);
        foreach (DataColumn column in result.Columns)
        {
            column.ColumnName = TemporaryName(result,
                names);
        }
        for (var index = 0; index < names.Length; index++)
        {
            result.Columns[index].ColumnName = names[index];
        }
        return result;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates an unused transient column name.
    /// </summary>
    /// <param name="table">The target table.</param>
    /// <param name="names">The reserved final names.</param>
    /// <returns>The temporary name.</returns>
    private static string TemporaryName(DataTable table,
        string[] names)
    {
        var index = 0;
        var name = "__RepoDb_0";
        while (table.Columns.Contains(name) || names
            .Contains(name,
                StringComparer.OrdinalIgnoreCase))
        {
            name = "__RepoDb_" + (++index).ToString(CultureInfo.InvariantCulture);
        }
        return name;
    }

    #endregion
}
