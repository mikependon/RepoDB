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
    /// Tests membership using CLR value equality and normalized nulls.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The target column.</param>
    /// <param name="values">The allowed values.</param>
    /// <returns>The operation result.</returns>
    public static bool[] IsIn(this DataTable table,
        string column,
        IEnumerable<object?> values)
    {
        var source = InspectionHelper.Column(table,
            column);
        ArgumentNullException.ThrowIfNull(values);
        var allowed = values
            .Select(value => InspectionHelper.IsMissing(value) ? null : value)
            .ToHashSet();
        return InspectionHelper.Rows(table)
            .Select(row => allowed.Contains(InspectionHelper.IsMissing(row[source]) ? null : row[source]))
            .ToArray();
    }

    #endregion
}
