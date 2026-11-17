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
    /// Converts selected finite numeric columns into a dense matrix.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="columns">The numeric column names, or all numeric columns.</param>
    /// <returns>The operation result.</returns>
    public static double[,] ToFeatureMatrix(this DataTable table,
        params string[] columns)
    {
        ArgumentNullException.ThrowIfNull(table);
        var selected = columns is { Length: 0 } ? table.Columns
            .Cast<DataColumn>()
            .Where(column => InspectionHelper.IsNumeric(column.DataType))
            .ToArray() : TableHelper.Columns(table,
                columns);
        return FeatureHelper.Matrix(table,
            selected);
    }

    #endregion
}
