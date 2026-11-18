using System.Data;

namespace RepoDb.Science;

/// <summary>
/// Provides analytical DataTable operations.
/// </summary>
public static partial class DataTableExtensions
{
    #region Public Methods

    /// <summary>
    /// Encodes categorical string columns as numeric indicators.
    /// </summary>
    /// <param name="table">The table to fit and transform.</param>
    /// <param name="columns">The categorical column names.</param>
    /// <returns>The encoded table.</returns>
    public static DataTable OneHotEncode(this DataTable table,
        params string[] columns)
    {
        return DataTableOneHotEncoder.Fit(table,
            columns)
            .Transform(table);
    }

    #endregion
}
