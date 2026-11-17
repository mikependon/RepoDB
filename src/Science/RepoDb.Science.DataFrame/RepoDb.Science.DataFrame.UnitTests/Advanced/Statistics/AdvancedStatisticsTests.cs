using System.Data;
using RepoDb.Science.UnitTests.Helpers;
using static RepoDb.Science.UnitTests.Helpers.AssertHelper;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Verifies statistics additions and failure behavior.
/// </summary>
internal static class AdvancedStatisticsTests
{
    #region Public Methods

    /// <summary>
    /// Runs the operation scenarios.
    /// </summary>
    public static void Run()
    {
        RunBatch1();
        RunBatch2();
        RunBatch3();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch1()
    {
        CountSupportsTextAndMissingValues();
        MedianAndQuantileInterpolate();
        QuantileHandlesEndpoints();
        QuantileAndDeviationHandleEmptyInput();
        SampleAndPopulationVarianceDiffer();
        SingletonDeviationDependsOnDegreesOfFreedom();
        ModeRetainsTiesAndFirstSeenOrder();
        ModeSupportsTextTies();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch2()
    {
        ModeCanIncludeMissingValues();
        EmptyModeReturnsNoValues();
        StatisticsSkipDeletedAndMissingValues();
        QuantileRejectsNegativeProbability();
        QuantileRejectsLargeProbability();
        QuantileRejectsNaN();
        QuantileRejectsInfinity();
        VarianceRejectsNegativeDegrees();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch3()
    {
        StatisticsRejectText();
    }

    /// <summary>
    /// Verifies count supports text and missing values.
    /// </summary>
    private static void CountSupportsTextAndMissingValues()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var text = table
            .Count("Key");
        var numeric = table
            .Count("Value");

        // Assert
        Check(text == 4 && numeric == 3);
    }

    /// <summary>
    /// Verifies median and quantile interpolate.
    /// </summary>
    private static void MedianAndQuantileInterpolate()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var median = table
            .Median("Count");
        var lower = table
            .Quantile("Count",
                0.25);
        var upper = table
            .Quantile("Count",
                1);

        // Assert
        Check(median == 2.5 && lower == 1.75 && upper == 3);
    }

    /// <summary>
    /// Verifies quantile handles endpoints.
    /// </summary>
    private static void QuantileHandlesEndpoints()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Quantile("Value",
                0);

        // Assert
        Check(result == 10);
    }

    /// <summary>
    /// Verifies quantile and deviation handle empty input.
    /// </summary>
    private static void QuantileAndDeviationHandleEmptyInput()
    {
        // Setup
        var table = TestTable.Create()
            .Clone();

        // Act
        var median = table
            .Median("Value");
        var std = table
            .Std("Value");
        var variance = table
            .Variance("Value",
                0);

        // Assert
        Check(median is null && std is null && variance is null);
    }

    /// <summary>
    /// Verifies sample and population variance differ.
    /// </summary>
    private static void SampleAndPopulationVarianceDiffer()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);

        // Act
        var sample = table
            .Variance("Id");
        var population = table
            .Variance("Id",
                0);
        var std = table
            .Std("Id");

        // Assert
        Check(sample == 1 && std == 1);
        Check(population.HasValue && Math.Abs(population.Value - 2.0 / 3) < 1e-12);
    }

    /// <summary>
    /// Verifies singleton deviation depends on degrees of freedom.
    /// </summary>
    private static void SingletonDeviationDependsOnDegreesOfFreedom()
    {
        // Setup
        var table = TestTable.Keys(5);

        // Act
        var sample = table
            .Std("Id");
        var population = table
            .Std("Id",
                0);

        // Assert
        Check(sample is null && population == 0);
    }

    /// <summary>
    /// Verifies mode retains ties and first seen order.
    /// </summary>
    private static void ModeRetainsTiesAndFirstSeenOrder()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Mode("Count");

        // Assert
        Check(result
            .SequenceEqual(new object?[] { 3 }));
    }

    /// <summary>
    /// Verifies mode supports text ties.
    /// </summary>
    private static void ModeSupportsTextTies()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Mode("Key");

        // Assert
        Check(result
            .SequenceEqual(new object?[] { "A", "B" }));
    }

    /// <summary>
    /// Verifies mode can include missing values.
    /// </summary>
    private static void ModeCanIncludeMissingValues()
    {
        // Setup
        var table = TestTable.Create();
        table.Rows[0]["Value"] = DBNull.Value;

        // Act
        var result = table
            .Mode("Value",
                true);

        // Assert
        Check(result
            .SequenceEqual(new object?[] { null, 30m }));
    }

    /// <summary>
    /// Verifies empty mode returns no values.
    /// </summary>
    private static void EmptyModeReturnsNoValues()
    {
        // Setup
        var table = TestTable.Create()
            .Clone();

        // Act
        var result = table
            .Mode("Value");

        // Assert
        Check(result.Count == 0);
    }

    /// <summary>
    /// Verifies statistics skip deleted and missing values.
    /// </summary>
    private static void StatisticsSkipDeletedAndMissingValues()
    {
        // Setup
        var table = TestTable.Create();
        table.AcceptChanges();
        table.Rows[0].Delete();

        // Act
        var result = table
            .Median("Value");

        // Assert
        Check(result == 30);
    }

    /// <summary>
    /// Verifies quantile rejects negative probability.
    /// </summary>
    private static void QuantileRejectsNegativeProbability()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Quantile("Count",
                -0.1);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies quantile rejects large probability.
    /// </summary>
    private static void QuantileRejectsLargeProbability()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Quantile("Count",
                1.1);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies quantile rejects na n.
    /// </summary>
    private static void QuantileRejectsNaN()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Quantile("Count",
                double.NaN);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies quantile rejects infinity.
    /// </summary>
    private static void QuantileRejectsInfinity()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Quantile("Count",
                double.PositiveInfinity);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies variance rejects negative degrees.
    /// </summary>
    private static void VarianceRejectsNegativeDegrees()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Variance("Count",
                -1);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies statistics reject text.
    /// </summary>
    private static void StatisticsRejectText()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Median("Key");

        // Assert
        Throws<ArgumentException>(operation);
    }

    #endregion
}
