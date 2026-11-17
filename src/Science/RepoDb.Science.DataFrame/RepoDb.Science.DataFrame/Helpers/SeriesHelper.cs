using System.Data;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Reads and writes nullable numeric series.
/// </summary>
internal static class SeriesHelper
{
    #region Public Methods

    /// <summary>
    /// Reads finite numeric values in current row order.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The numeric column.</param>
    /// <returns>The numeric values with missing entries preserved.</returns>
    public static double?[] Read(DataTable table,
        string column)
    {
        var source = InspectionHelper.Column(table,
            column);
        FeatureHelper.RequireNumeric(source);
        return InspectionHelper.Rows(table)
            .Select(row => InspectionHelper.IsMissing(row[source]) ? (double?)null : FeatureHelper.Number(row[source]))
            .ToArray();
    }

    /// <summary>
    /// Writes a nullable double column without converting missing values to zero.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The output column.</param>
    /// <param name="values">The computed values.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The transformed table.</returns>
    public static DataTable Write(DataTable table,
        string column,
        double?[] values,
        bool inPlace)
    {
        var boxed = values
            .Select(value => value.HasValue ? (object)Finite(value.Value) : DBNull.Value)
            .ToArray();
        return TransformationHelper.SetColumn(table,
            column,
            typeof(double),
            boxed,
            inPlace);
    }

    /// <summary>
    /// Rejects non-finite computed values.
    /// </summary>
    /// <param name="value">The computed value.</param>
    /// <returns>The finite value.</returns>
    public static double Finite(double value)
    {
        if (!double.IsFinite(value))
        {
            throw new ArithmeticException("The numeric result exceeds finite double precision.");
        }
        return value;
    }

    /// <summary>
    /// Extracts present values from a series.
    /// </summary>
    /// <param name="values">The nullable values.</param>
    /// <returns>The present values.</returns>
    public static double[] Present(IEnumerable<double?> values)
    {
        return values
            .Where(value => value.HasValue)
            .Select(value => value.GetValueOrDefault())
            .ToArray();
    }

    #endregion
}
