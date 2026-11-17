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
    /// Computes a new or replacement column.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The output column name.</param>
    /// <param name="selector">The row computation.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>The operation result.</returns>
    public static DataTable Assign<T>(this DataTable table,
        string column,
        Func<DataRow, T> selector,
        bool inPlace = false)
    {
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentException.ThrowIfNullOrWhiteSpace(column);
        var values = InspectionHelper.Rows(table)
            .Select(row => (object?)selector(row))
            .ToArray();
        return TransformationHelper.SetColumn(table,
            column,
            typeof(T),
            values,
            inPlace);
    }

    #endregion
}
