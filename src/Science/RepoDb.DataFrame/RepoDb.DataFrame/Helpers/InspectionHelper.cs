using System.Data;
using System.Globalization;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Provides shared inspection operations.
/// </summary>
internal static class InspectionHelper
{
    #region Public Methods

    /// <summary>
    /// Gets rows with current values.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <returns>The current rows.</returns>
    public static IEnumerable<DataRow> Rows(DataTable table)
    {
        ArgumentNullException.ThrowIfNull(table);
        return table.Rows
            .Cast<DataRow>()
            .Where(row => row.RowState != DataRowState.Deleted);
    }

    /// <summary>
    /// Resolves a column name.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="name">The column name.</param>
    /// <returns>The column.</returns>
    public static DataColumn Column(DataTable table,
        string name)
    {
        ArgumentNullException.ThrowIfNull(table);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return table.Columns[name] ?? throw new ArgumentException("Column does not exist.",
            nameof(name));
    }

    /// <summary>
    /// Copies selected current values and the table schema.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="rows">The selected rows.</param>
    /// <returns>The independent table.</returns>
    public static DataTable CopyRows(DataTable table,
        IEnumerable<DataRow> rows)
    {
        ArgumentNullException.ThrowIfNull(table);
        var result = table.Clone();
        foreach (var row in rows)
        {
            result.ImportRow(row);
        }
        return result;
    }

    /// <summary>
    /// Partially shuffles rows without replacement.
    /// </summary>
    /// <param name="rows">The row buffer.</param>
    /// <param name="count">The sample size.</param>
    /// <param name="random">The random generator.</param>
    public static void Shuffle(DataRow[] rows,
        int count,
        Random random)
    {
        for (var index = 0; index < count; index++)
        {
            var selected = random.Next(index,
                rows.Length);
            (rows[index],
                rows[selected]) = (rows[selected],
                rows[index]);
        }
    }

    /// <summary>
    /// Checks whether a value is missing.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>Whether the value is missing.</returns>
    public static bool IsMissing(object? value) => value is null or DBNull;

    /// <summary>
    /// Creates a column metadata snapshot.
    /// </summary>
    /// <param name="column">The column.</param>
    /// <param name="rows">The current rows.</param>
    /// <returns>The metadata.</returns>
    public static ColumnInfo GetColumnInfo(DataColumn column,
        DataRow[] rows)
    {
        var missing = rows
            .Count(row => IsMissing(row[column]));
        return new ColumnInfo(column.ColumnName,
            column.DataType,
            column.AllowDBNull,
            missing,
            rows.Length - missing);
    }

    /// <summary>
    /// Checks supported numeric types.
    /// </summary>
    /// <param name="type">The declared type.</param>
    /// <returns>Whether the type is numeric.</returns>
    public static bool IsNumeric(Type type)
    {
        return !type.IsEnum && Type.GetTypeCode(type) is TypeCode.Byte or TypeCode.SByte
            or TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32 or TypeCode.UInt32
            or TypeCode.Int64 or TypeCode.UInt64 or TypeCode.Single or TypeCode.Double or TypeCode.Decimal;
    }

    /// <summary>
    /// Computes statistics for one numeric column.
    /// </summary>
    /// <param name="column">The column.</param>
    /// <param name="rows">The current rows.</param>
    /// <returns>The statistics.</returns>
    public static ColumnStatistics DescribeColumn(DataColumn column,
        DataRow[] rows)
    {
        var values = rows
            .Select(row => row[column])
            .Where(value => !IsMissing(value))
            .Select(NumericValue)
            .Order()
            .ToArray();
        var (mean,
            deviation) = Moments(values);
        return new ColumnStatistics(column.ColumnName,
            values.Length,
            mean,
            deviation,
            Quantile(values,
                0),
            Quantile(values,
                0.25),
            Quantile(values,
                0.5),
            Quantile(values,
                0.75),
            Quantile(values,
                1));
    }

    /// <summary>
    /// Estimates a value's stored payload size.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The estimated bytes.</returns>
    public static long ValueSize(object? value)
    {
        return value switch
        {
            null or DBNull => 0,
            string text => (long)text.Length * sizeof(char),
            byte[] bytes => bytes.LongLength,
            bool or byte or sbyte => 1,
            char or short or ushort => 2,
            int or uint or float => 4,
            long or ulong or double or DateTime or TimeSpan => 8,
            decimal or Guid or DateTimeOffset => 16,
            _ => IntPtr.Size
        };
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Converts finite numeric values to double precision.
    /// </summary>
    /// <param name="value">The numeric value.</param>
    /// <returns>The converted value.</returns>
    private static double NumericValue(object value)
    {
        var number = Convert.ToDouble(value,
            CultureInfo.InvariantCulture);
        if (!double.IsFinite(number))
        {
            throw new ArgumentException("Describe requires finite numeric values.",
                nameof(value));
        }
        return number;
    }

    /// <summary>
    /// Computes stable mean and sample deviation.
    /// </summary>
    /// <param name="values">The numeric values.</param>
    /// <returns>The mean and deviation.</returns>
    private static (double? Mean, double? Deviation) Moments(double[] values)
    {
        double mean = 0, squaredDifferences = 0;
        for (var index = 0; index < values.Length; index++)
        {
            var delta = values[index] - mean;
            mean += delta / (index + 1);
            squaredDifferences += delta * (values[index] - mean);
        }
        return (values.Length == 0 ? null : mean,
            values.Length < 2 ? null : Math.Sqrt(squaredDifferences / (values.Length - 1)));
    }

    /// <summary>
    /// Computes a linearly interpolated quantile.
    /// </summary>
    /// <param name="values">The sorted values.</param>
    /// <param name="fraction">The quantile fraction.</param>
    /// <returns>The quantile, or null for empty input.</returns>
    private static double? Quantile(double[] values,
        double fraction)
    {
        if (values.Length == 0)
        {
            return null;
        }
        var position = (values.Length - 1) * fraction;
        var lower = (int)position;
        var upper = (int)Math.Ceiling(position);
        var weight = position - lower;
        return lower == upper ? values[lower] :
            values[lower] * (1 - weight) + values[upper] * weight;
    }

    #endregion
}
