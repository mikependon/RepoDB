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
    /// Linearly fills bounded numeric gaps by row position.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The numeric column.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The operation result.</returns>
    public static DataTable Interpolate(this DataTable table,
        string column,
        bool inPlace = false)
    {
        var values = SeriesHelper.Read(table,
            column);
        SequenceHelper.Interpolate(values);
        return SeriesHelper.Write(table,
            column,
            values,
            inPlace);
    }

    #endregion
}
