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
    /// Rebuilds a column using invariant numeric or string conversion.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The column to convert.</param>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>The operation result.</returns>
    public static DataTable CastColumn<T>(this DataTable table,
        string column)
    {
        var source = InspectionHelper.Column(table,
            column);
        var values = InspectionHelper.Rows(table)
            .Select(row => TransformationHelper.ConvertValue(row[source],
                typeof(T)))
                .ToArray();
        return TransformationHelper.SetColumn(table,
            source.ColumnName,
            typeof(T),
            values,
            false);
    }

    #endregion
}
