using System.Data;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// Inspects tables without modifying their contents.
/// </summary>
public static partial class DataTableExtensions
{
    #region Public Methods

    /// <summary>
    /// Describes finite numeric columns using double precision and sample deviation.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <returns>Numeric summaries in column order.</returns>
    public static IReadOnlyList<ColumnStatistics> Describe(this DataTable table)
    {
        var rows = InspectionHelper.Rows(table)
            .ToArray();
        return table.Columns
            .Cast<DataColumn>()
            .Where(column => InspectionHelper.IsNumeric(column.DataType))
            .Select(column => InspectionHelper.DescribeColumn(column,
                rows))
            .ToList()
            .AsReadOnly();
    }

    #endregion
}
