using System.Data;
using RepoDb.Science.Helpers;
using static RepoDb.Science.UnitTests.Helpers.AssertHelper;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Verifies numeric boundaries and shared summary contracts.
/// </summary>
internal static class AdvancedHelperTests
{
    #region Public Methods

    /// <summary>
    /// Runs numeric boundary scenarios.
    /// </summary>
    public static void Run()
    {
        SummaryRejectsUnknownFunction();
        VarianceRemainsStableWithLargeOffsets();
        InterpolationHandlesOpposingLargeEndpoints();
        QuantileHandlesLargeEqualEndpoints();
        EmptyMasksAndWindowsRetainTheirShapes();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Checks invalid function validation independently of window factories.
    /// </summary>
    private static void SummaryRejectsUnknownFunction()
    {
        // Setup
        var values = Array.Empty<double>();

        // Act
        Action operation = () => StatisticsHelper.Summary(values,
            (AggregateFunction)999);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Checks stable variance for small deviations from a large baseline.
    /// </summary>
    private static void VarianceRemainsStableWithLargeOffsets()
    {
        // Setup
        var table = CreateTable(1e12 + 1,
            1e12 + 2,
            1e12 + 3);

        // Act
        var result = table
            .Variance("Value");

        // Assert
        Check(result == 1);
    }

    /// <summary>
    /// Checks interpolation without subtracting opposite extreme endpoints.
    /// </summary>
    private static void InterpolationHandlesOpposingLargeEndpoints()
    {
        // Setup
        var table = CreateTable(-double.MaxValue,
            DBNull.Value,
            double.MaxValue);

        // Act
        var result = table
            .Interpolate("Value");

        // Assert
        Check((double)result.Rows[1][0] == 0);
    }

    /// <summary>
    /// Checks interpolation weights for large equal numeric quantiles.
    /// </summary>
    private static void QuantileHandlesLargeEqualEndpoints()
    {
        // Setup
        var table = CreateTable(double.MaxValue,
            double.MaxValue);

        // Act
        var result = table
            .Quantile("Value",
                0.5);

        // Assert
        Check(result == double.MaxValue);
    }

    /// <summary>
    /// Checks zero-row masks and window results.
    /// </summary>
    private static void EmptyMasksAndWindowsRetainTheirShapes()
    {
        // Setup
        var table = CreateTable();

        // Act
        var mask = table
            .IsNull("Value");
        var duplicate = table
            .Duplicated();
        var weighted = table
            .Ewm("Value",
                0.5)
            .Mean();

        // Assert
        Check(mask.Length == 0 && duplicate.Length == 0);
        Check(weighted.Rows.Count == 0 && weighted.Columns.Count == 1);
    }

    /// <summary>
    /// Creates a nullable double-valued series.
    /// </summary>
    /// <param name="values">The current values.</param>
    /// <returns>The source table.</returns>
    private static DataTable CreateTable(params object[] values)
    {
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(double));
        foreach (var value in values)
        {
            table.Rows.Add(value);
        }
        return table;
    }

    #endregion
}
