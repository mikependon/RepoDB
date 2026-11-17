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
    /// Computes absolute values with checked conversion to the stored type.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The numeric column.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The operation result.</returns>
    public static DataTable Abs(this DataTable table,
        string column,
        bool inPlace = false)
    {
        return NumericTransformationHelper.Apply(table,
            column,
            decimal.Abs,
            Math.Abs,
            inPlace);
    }

    #endregion
}
