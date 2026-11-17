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
    /// Maps present column values while preserving missing values.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The target column.</param>
    /// <param name="mapper">The value computation.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <typeparam name="TSource">The value type.</typeparam>
    /// <typeparam name="TResult">The value type.</typeparam>
    /// <returns>The operation result.</returns>
    public static DataTable MapColumn<TSource, TResult>(this DataTable table,
        string column,
        Func<TSource, TResult> mapper,
        bool inPlace = false)
    {
        var source = InspectionHelper.Column(table,
            column);
        ArgumentNullException.ThrowIfNull(mapper);
        var values = InspectionHelper.Rows(table)
            .Select(row => InspectionHelper.IsMissing(row[source]) ? null : (object?)mapper((TSource)row[source]))
            .ToArray();
        return TransformationHelper.SetColumn(table,
            source.ColumnName,
            typeof(TResult),
            values,
            inPlace);
    }

    #endregion
}
