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
    /// Shifts current values by a signed row offset.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The target column.</param>
    /// <param name="periods">The signed row offset.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The operation result.</returns>
    public static DataTable Shift(this DataTable table,
        string column,
        int periods = 1,
        bool inPlace = false)
    {
        var source = InspectionHelper.Column(table,
            column);
        var values = InspectionHelper.Rows(table)
            .Select(row => (object?)row[source])
            .ToArray();
        var shifted = SequenceHelper.Shift(values,
            periods);
        return TransformationHelper.SetColumn(table,
            source.ColumnName,
            source.DataType,
            shifted,
            inPlace);
    }

    #endregion
}
