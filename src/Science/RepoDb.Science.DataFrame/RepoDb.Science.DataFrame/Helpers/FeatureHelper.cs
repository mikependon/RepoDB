using System.Data;
using System.Globalization;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Converts numeric rows into machine learning inputs.
/// </summary>
internal static class FeatureHelper
{
    #region Public Methods

    /// <summary>
    /// Converts numeric columns into an ordered matrix.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="columns">The ordered feature columns.</param>
    /// <returns>The dense matrix.</returns>
    public static double[,] Matrix(DataTable table,
        DataColumn[] columns)
    {
        foreach (var column in columns)
        {
            RequireNumeric(column);
        }
        var rows = InspectionHelper.Rows(table)
            .ToArray();
        var result = new double[rows.Length, columns.Length];
        for (var row = 0; row < rows.Length; row++)
        {
            for (var column = 0; column < columns.Length; column++)
            {
                result[row, column] = Number(rows[row][columns[column]]);
            }
        }
        return result;
    }

    /// <summary>
    /// Requires a declared numeric column.
    /// </summary>
    /// <param name="column">The input column.</param>
    public static void RequireNumeric(DataColumn column)
    {
        if (!InspectionHelper.IsNumeric(column.DataType))
        {
            throw new ArgumentException("A numeric column is required.",
                nameof(column));
        }
    }

    /// <summary>
    /// Converts a present finite number to double precision.
    /// </summary>
    /// <param name="value">The input value.</param>
    /// <returns>The numeric value.</returns>
    public static double Number(object value)
    {
        if (InspectionHelper.IsMissing(value))
        {
            throw new InvalidOperationException("Missing features require explicit imputation.");
        }
        var result = Convert.ToDouble(value,
            CultureInfo.InvariantCulture);
        if (!double.IsFinite(result))
        {
            throw new ArgumentException("Finite numeric values are required.",
                nameof(value));
        }
        return result;
    }

    #endregion
}
