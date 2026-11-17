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
    /// Materializes an explicitly keyed reshaped table.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="idColumns">The retained identifier names.</param>
    /// <param name="valueColumns">The value names, or all remaining columns.</param>
    /// <param name="variableName">The variable output name.</param>
    /// <param name="valueName">The value output name.</param>
    /// <returns>The operation result.</returns>
    public static DataTable Melt(this DataTable table,
        string[] idColumns,
        string[]? valueColumns = null,
        string variableName = "Variable",
        string valueName = "Value")
    {
        return ReshapeHelper.Melt(table,
            idColumns,
            valueColumns,
            variableName,
            valueName);
    }

    #endregion
}
