using System.Data;
using System.Globalization;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Applies numeric functions while preserving stored CLR types.
/// </summary>
internal static class NumericTransformationHelper
{
    #region Public Methods

    /// <summary>
    /// Rejects fractional clipping bounds for integral output columns.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The numeric column.</param>
    /// <param name="lower">The optional lower bound.</param>
    /// <param name="upper">The optional upper bound.</param>
    public static void ValidateClip(DataTable table,
        string column,
        decimal? lower,
        decimal? upper)
    {
        var source = InspectionHelper.Column(table,
            column);
        FeatureHelper.RequireNumeric(source);
        var integral = source.DataType != typeof(float) && source.DataType != typeof(double) && source.DataType != typeof(decimal);
        if (integral && new[] { lower, upper }
            .Any(bound => bound.HasValue && decimal.Truncate(bound.Value) != bound.Value))
        {
            throw new ArgumentException("Integral columns require integral clipping bounds.",
                nameof(lower));
        }
    }

    /// <summary>
    /// Applies decimal or floating-point arithmetic to present values.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The numeric column.</param>
    /// <param name="exact">The integer and decimal computation.</param>
    /// <param name="floating">The floating-point computation.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The transformed table.</returns>
    public static DataTable Apply(DataTable table,
        string column,
        Func<decimal, decimal> exact,
        Func<double, double> floating,
        bool inPlace)
    {
        var source = InspectionHelper.Column(table,
            column);
        FeatureHelper.RequireNumeric(source);
        var useDouble = source.DataType == typeof(double) || source.DataType == typeof(float);
        var values = InspectionHelper.Rows(table)
            .Select(row => ConvertValue(row[source],
                source.DataType,
                useDouble,
                exact,
                floating))
            .ToArray();
        return TransformationHelper.SetColumn(table,
            source.ColumnName,
            source.DataType,
            values,
            inPlace);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Computes and converts one present numeric value.
    /// </summary>
    /// <param name="value">The input value.</param>
    /// <param name="type">The stored type.</param>
    /// <param name="useDouble">Whether to use floating arithmetic.</param>
    /// <param name="exact">The exact computation.</param>
    /// <param name="floating">The floating computation.</param>
    /// <returns>The transformed value or DBNull.</returns>
    private static object ConvertValue(object value,
        Type type,
        bool useDouble,
        Func<decimal, decimal> exact,
        Func<double, double> floating)
    {
        if (InspectionHelper.IsMissing(value))
        {
            return DBNull.Value;
        }
        object result = useDouble ? (object)SeriesHelper.Finite(floating(FeatureHelper.Number(value))) :
            exact(Convert.ToDecimal(value,
                CultureInfo.InvariantCulture));
        return TransformationHelper.ConvertValue(result,
            type);
    }

    #endregion
}
