using System.Data;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Computes pairwise-complete covariance and Pearson correlation.
/// </summary>
internal static class CorrelationHelper
{
    #region Public Methods

    /// <summary>
    /// Computes a paired summary from nullable numeric observations.
    /// </summary>
    /// <param name="left">The left series.</param>
    /// <param name="right">The right series.</param>
    /// <param name="correlation">Whether to return Pearson correlation.</param>
    /// <param name="degreesOfFreedom">The covariance denominator adjustment.</param>
    /// <returns>The summary or null for insufficient or constant observations.</returns>
    public static double? Calculate(double?[] left,
        double?[] right,
        bool correlation,
        int degreesOfFreedom)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(degreesOfFreedom);
        var pairs = left
            .Zip(right)
            .Where(pair => pair.First.HasValue && pair.Second.HasValue)
            .Select(pair => (X: pair.First!.Value,
                Y: pair.Second!.Value))
            .ToArray();
        if (pairs.Length <= degreesOfFreedom)
        {
            return null;
        }
        var (coMoment,
            xSquares,
            ySquares) = Moments(pairs);
        if (!correlation)
        {
            return SeriesHelper.Finite(coMoment / (pairs.Length - degreesOfFreedom));
        }
        return xSquares == 0 || ySquares == 0 ? null :
            Math.Clamp(SeriesHelper.Finite(coMoment / Math.Sqrt(xSquares) / Math.Sqrt(ySquares)),
                -1,
                1);
    }

    /// <summary>
    /// Computes a labeled correlation matrix.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="names">The selected names, or all numeric columns.</param>
    /// <returns>The labeled matrix table.</returns>
    public static DataTable Matrix(DataTable table,
        string[] names)
    {
        var columns = TableHelper.Columns(table,
            names,
            true);
        var selected = names.Length == 0 ? columns
            .Where(column => InspectionHelper.IsNumeric(column.DataType))
            .ToArray() : columns;
        var series = selected
            .Select(column => SeriesHelper.Read(table,
                column.ColumnName))
            .ToArray();
        var result = MatrixSchema(table,
            selected);
        for (var index = 0; index < selected.Length; index++)
        {
            var row = result.NewRow();
            row[0] = selected[index].ColumnName;
            for (var other = 0; other < selected.Length; other++)
            {
                row[other + 1] = (object?)Calculate(series[index],
                    series[other],
                    true,
                    1) ?? DBNull.Value;
            }
            result.Rows.Add(row);
        }
        return result;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Computes stable paired moments.
    /// </summary>
    /// <param name="pairs">The present paired observations.</param>
    /// <returns>The cross moment and squared deviations.</returns>
    private static (double Cross, double XSquares, double YSquares) Moments((double X, double Y)[] pairs)
    {
        double meanX = 0, meanY = 0, cross = 0, xSquares = 0, ySquares = 0;
        var count = 0;
        foreach (var pair in pairs)
        {
            var dx = pair.X - meanX;
            var dy = pair.Y - meanY;
            meanX += dx / ++count;
            meanY += dy / count;
            cross += dx * (pair.Y - meanY);
            xSquares += dx * (pair.X - meanX);
            ySquares += dy * (pair.Y - meanY);
        }
        return (cross,
            xSquares,
            ySquares);
    }

    /// <summary>
    /// Creates a matrix schema with a reserved row-label column.
    /// </summary>
    /// <param name="source">The source table.</param>
    /// <param name="columns">The numeric columns.</param>
    /// <returns>The empty matrix schema.</returns>
    private static DataTable MatrixSchema(DataTable source,
        DataColumn[] columns)
    {
        var result = TableHelper.Schema(source,
            Array.Empty<DataColumn>());
        TableHelper.AddColumn(result,
            "Column",
            typeof(string));
        foreach (var column in columns)
        {
            TableHelper.AddColumn(result,
                column.ColumnName,
                typeof(double));
        }
        return result;
    }

    #endregion
}
