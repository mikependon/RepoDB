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
    /// Computes differences against a signed preceding row offset.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The numeric column.</param>
    /// <param name="periods">The signed row offset.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The operation result.</returns>
    public static DataTable Diff(this DataTable table,
        string column,
        int periods = 1,
        bool inPlace = false)
    {
        var values = SeriesHelper.Read(table,
            column);
        var result = SequenceHelper.Compare(values,
            periods,
            false);
        return SeriesHelper.Write(table,
            column,
            result,
            inPlace);
    }

    #endregion
}
