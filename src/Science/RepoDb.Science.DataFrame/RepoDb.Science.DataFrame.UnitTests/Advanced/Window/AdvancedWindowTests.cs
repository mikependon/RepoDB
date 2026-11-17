using System.Data;
using RepoDb.Science.UnitTests.Helpers;
using static RepoDb.Science.UnitTests.Helpers.AssertHelper;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Verifies window additions and failure behavior.
/// </summary>
internal static class AdvancedWindowTests
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
        RunBatch4();
        RunBatch5();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch1()
    {
        RollingRequiresFullWidthByDefault();
        RollingCountsPresentObservations();
        RollingSumSummarizesTrailingRows();
        RollingMinSummarizesTrailingRows();
        RollingMaxSummarizesTrailingRows();
        RollingMeanSummarizesTrailingRows();
        RollingCountSummarizesTrailingRows();
        RollingVarianceUsesSampleDenominator();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch2()
    {
        RollingVarianceSupportsPopulation();
        ExpandingAccumulatesFromFirstRow();
        WindowCanWriteInPlaceOutputColumn();
        WindowStdCanWriteInPlace();
        WindowReadsCurrentDataAtExecution();
        EmptyObservationMeanHandlesZeroMinimum();
        EmptyObservationMinHandlesZeroMinimum();
        EmptyObservationMaxHandlesZeroMinimum();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch3()
    {
        EmptyObservationCountHandlesZeroMinimum();
        EmptyObservationSumHandlesZeroMinimum();
        EmptyObservationStdHandlesZeroMinimum();
        EmptyObservationVarianceHandlesZeroMinimum();
        WindowSupportsEmptyTable();
        EwmComputesRecursiveMeans();
        EwmIgnoresMissingValuesAndCarriesMean();
        EwmEnforcesMinimumPresentCount();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch4()
    {
        EwmSupportsFullWeightAndEmptyTable();
        RollingRejectsZeroWidth();
        RollingRejectsNegativeMinimum();
        RollingRejectsExcessMinimum();
        ExpandingRejectsNegativeMinimum();
        EwmRejectsZeroAlpha();
        EwmRejectsNegativeAlpha();
        EwmRejectsExcessAlpha();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch5()
    {
        EwmRejectsNaNAlpha();
        EwmRejectsInfiniteAlpha();
        EwmRejectsZeroMinimum();
        WindowRejectsNegativeVarianceAdjustment();
        WindowRejectsNegativeDeviationAdjustment();
        WindowRejectsUnknownAggregate();
        WindowRejectsNonnumericSource();
    }

    /// <summary>
    /// Verifies rolling requires full width by default.
    /// </summary>
    private static void RollingRequiresFullWidthByDefault()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3,
            4);

        // Act
        var result = table
            .Rolling("Id",
                3)
            .Mean();

        // Assert
        Check(result.Rows[0]
            .IsNull(0) && result.Rows[1]
            .IsNull(0));
        Check((double)result.Rows[2][0] == 2 && (double)result.Rows[3][0] == 3);
    }

    /// <summary>
    /// Verifies rolling counts present observations.
    /// </summary>
    private static void RollingCountsPresentObservations()
    {
        // Setup
        var table = TestTable.Keys(1,
            DBNull.Value,
            3);

        // Act
        var result = table
            .Rolling("Id",
                2,
                1)
            .Count();

        // Assert
        Check((double)result.Rows[0][0] == 1 && (double)result.Rows[1][0] == 1 && (double)result.Rows[2][0] == 1);
    }

    /// <summary>
    /// Verifies rolling sum summarizes trailing rows.
    /// </summary>
    private static void RollingSumSummarizesTrailingRows()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);

        // Act
        var result = table
            .Rolling("Id",
                2)
            .Sum();

        // Assert
        Check((double)result.Rows[2][0] == 5);
    }

    /// <summary>
    /// Verifies rolling min summarizes trailing rows.
    /// </summary>
    private static void RollingMinSummarizesTrailingRows()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);

        // Act
        var result = table
            .Rolling("Id",
                2)
            .Min();

        // Assert
        Check((double)result.Rows[2][0] == 2);
    }

    /// <summary>
    /// Verifies rolling max summarizes trailing rows.
    /// </summary>
    private static void RollingMaxSummarizesTrailingRows()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);

        // Act
        var result = table
            .Rolling("Id",
                2)
            .Max();

        // Assert
        Check((double)result.Rows[2][0] == 3);
    }

    /// <summary>
    /// Verifies rolling mean summarizes trailing rows.
    /// </summary>
    private static void RollingMeanSummarizesTrailingRows()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);

        // Act
        var result = table
            .Rolling("Id",
                2)
            .Mean();

        // Assert
        Check((double)result.Rows[2][0] == 2.5);
    }

    /// <summary>
    /// Verifies rolling count summarizes trailing rows.
    /// </summary>
    private static void RollingCountSummarizesTrailingRows()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);

        // Act
        var result = table
            .Rolling("Id",
                2)
            .Count();

        // Assert
        Check((double)result.Rows[2][0] == 2);
    }

    /// <summary>
    /// Verifies rolling variance uses sample denominator.
    /// </summary>
    private static void RollingVarianceUsesSampleDenominator()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);

        // Act
        var variance = table
            .Rolling("Id",
                2)
            .Variance();
        var deviation = table
            .Rolling("Id",
                2)
            .Std();

        // Assert
        Check((double)variance.Rows[1][0] == 0.5);
        Check(Math.Abs((double)deviation.Rows[2][0] - Math.Sqrt(0.5)) < 1e-12);
    }

    /// <summary>
    /// Verifies rolling variance supports population.
    /// </summary>
    private static void RollingVarianceSupportsPopulation()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);

        // Act
        var result = table
            .Rolling("Id",
                2)
            .Variance(0);

        // Assert
        Check((double)result.Rows[1][0] == 0.25);
    }

    /// <summary>
    /// Verifies expanding accumulates from first row.
    /// </summary>
    private static void ExpandingAccumulatesFromFirstRow()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);

        // Act
        var result = table
            .Expanding("Id")
            .Sum("Running");

        // Assert
        Check((double)result.Rows[2]["Running"] == 6 && table.Columns.Count == 1);
    }

    /// <summary>
    /// Verifies window can write in place output column.
    /// </summary>
    private static void WindowCanWriteInPlaceOutputColumn()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);

        // Act
        var result = table
            .Rolling("Id",
                2)
            .Mean("Average",
                true);

        // Assert
        Check(ReferenceEquals(table,
            result) && (double)table.Rows[2]["Average"] == 2.5);
    }

    /// <summary>
    /// Verifies window std can write in place.
    /// </summary>
    private static void WindowStdCanWriteInPlace()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);

        // Act
        var result = table
            .Expanding("Id")
            .Std(outputColumn: "Deviation",
                inPlace: true);

        // Assert
        Check(ReferenceEquals(table,
            result) && table.Rows[0]
            .IsNull("Deviation") && (double)table.Rows[2]["Deviation"] == 1);
    }

    /// <summary>
    /// Verifies window reads current data at execution.
    /// </summary>
    private static void WindowReadsCurrentDataAtExecution()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        var window = table
            .Expanding("Id");
        table.Rows[0][0] = 3;
        var result = window
            .Sum();

        // Assert
        Check((double)result.Rows[1][0] == 5);
    }

    /// <summary>
    /// Verifies empty observation mean handles zero minimum.
    /// </summary>
    private static void EmptyObservationMeanHandlesZeroMinimum()
    {
        // Setup
        var table = TestTable.Keys(DBNull.Value);

        // Act
        var result = table
            .Rolling("Id",
                1,
                0)
            .Mean();

        // Assert
        Check(result.Rows[0]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies empty observation min handles zero minimum.
    /// </summary>
    private static void EmptyObservationMinHandlesZeroMinimum()
    {
        // Setup
        var table = TestTable.Keys(DBNull.Value);

        // Act
        var result = table
            .Rolling("Id",
                1,
                0)
            .Min();

        // Assert
        Check(result.Rows[0]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies empty observation max handles zero minimum.
    /// </summary>
    private static void EmptyObservationMaxHandlesZeroMinimum()
    {
        // Setup
        var table = TestTable.Keys(DBNull.Value);

        // Act
        var result = table
            .Rolling("Id",
                1,
                0)
            .Max();

        // Assert
        Check(result.Rows[0]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies empty observation count handles zero minimum.
    /// </summary>
    private static void EmptyObservationCountHandlesZeroMinimum()
    {
        // Setup
        var table = TestTable.Keys(DBNull.Value);

        // Act
        var result = table
            .Rolling("Id",
                1,
                0)
            .Count();

        // Assert
        Check((double)result.Rows[0][0] == 0);
    }

    /// <summary>
    /// Verifies empty observation sum handles zero minimum.
    /// </summary>
    private static void EmptyObservationSumHandlesZeroMinimum()
    {
        // Setup
        var table = TestTable.Keys(DBNull.Value);

        // Act
        var result = table
            .Rolling("Id",
                1,
                0)
            .Sum();

        // Assert
        Check((double)result.Rows[0][0] == 0);
    }

    /// <summary>
    /// Verifies empty observation std handles zero minimum.
    /// </summary>
    private static void EmptyObservationStdHandlesZeroMinimum()
    {
        // Setup
        var table = TestTable.Keys(DBNull.Value);

        // Act
        var result = table
            .Rolling("Id",
                1,
                0)
            .Std();

        // Assert
        Check(result.Rows[0]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies empty observation variance handles zero minimum.
    /// </summary>
    private static void EmptyObservationVarianceHandlesZeroMinimum()
    {
        // Setup
        var table = TestTable.Keys(DBNull.Value);

        // Act
        var result = table
            .Rolling("Id",
                1,
                0)
            .Variance();

        // Assert
        Check(result.Rows[0]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies window supports empty table.
    /// </summary>
    private static void WindowSupportsEmptyTable()
    {
        // Setup
        var table = TestTable.Keys();

        // Act
        var result = table
            .Rolling("Id",
                2)
            .Mean();

        // Assert
        Check(result.Rows.Count == 0);
    }

    /// <summary>
    /// Verifies ewm computes recursive means.
    /// </summary>
    private static void EwmComputesRecursiveMeans()
    {
        // Setup
        var table = TestTable.Keys(1,
            3,
            5);

        // Act
        var result = table
            .Ewm("Id",
                0.5)
            .Mean();

        // Assert
        Check((double)result.Rows[0][0] == 1 && (double)result.Rows[1][0] == 2 && (double)result.Rows[2][0] == 3.5);
    }

    /// <summary>
    /// Verifies ewm ignores missing values and carries mean.
    /// </summary>
    private static void EwmIgnoresMissingValuesAndCarriesMean()
    {
        // Setup
        var table = TestTable.Keys(DBNull.Value,
            1,
            DBNull.Value,
            3);

        // Act
        var result = table
            .Ewm("Id",
                0.5)
            .Mean();

        // Assert
        Check(result.Rows[0]
            .IsNull(0) && (double)result.Rows[2][0] == 1 && (double)result.Rows[3][0] == 2);
    }

    /// <summary>
    /// Verifies ewm enforces minimum present count.
    /// </summary>
    private static void EwmEnforcesMinimumPresentCount()
    {
        // Setup
        var table = TestTable.Keys(1,
            DBNull.Value,
            3);

        // Act
        var result = table
            .Ewm("Id",
                0.5,
                2)
            .Mean("Weighted",
                true);

        // Assert
        Check(ReferenceEquals(table,
            result) && result.Rows[1]
            .IsNull("Weighted") && (double)result.Rows[2]["Weighted"] == 2);
    }

    /// <summary>
    /// Verifies ewm supports full weight and empty table.
    /// </summary>
    private static void EwmSupportsFullWeightAndEmptyTable()
    {
        // Setup
        var table = TestTable.Keys();

        // Act
        var result = table
            .Ewm("Id",
                1)
            .Mean();

        // Assert
        Check(result.Rows.Count == 0);
    }

    /// <summary>
    /// Verifies rolling rejects zero width.
    /// </summary>
    private static void RollingRejectsZeroWidth()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        Action operation = () => table
            .Rolling("Id",
                0);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies rolling rejects negative minimum.
    /// </summary>
    private static void RollingRejectsNegativeMinimum()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        Action operation = () => table
            .Rolling("Id",
                2,
                -1);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies rolling rejects excess minimum.
    /// </summary>
    private static void RollingRejectsExcessMinimum()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        Action operation = () => table
            .Rolling("Id",
                2,
                3);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies expanding rejects negative minimum.
    /// </summary>
    private static void ExpandingRejectsNegativeMinimum()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        Action operation = () => table
            .Expanding("Id",
                -1);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies ewm rejects zero alpha.
    /// </summary>
    private static void EwmRejectsZeroAlpha()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        Action operation = () => table
            .Ewm("Id",
                0);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies ewm rejects negative alpha.
    /// </summary>
    private static void EwmRejectsNegativeAlpha()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        Action operation = () => table
            .Ewm("Id",
                -0.1);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies ewm rejects excess alpha.
    /// </summary>
    private static void EwmRejectsExcessAlpha()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        Action operation = () => table
            .Ewm("Id",
                1.1);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies ewm rejects na nalpha.
    /// </summary>
    private static void EwmRejectsNaNAlpha()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        Action operation = () => table
            .Ewm("Id",
                double.NaN);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies ewm rejects infinite alpha.
    /// </summary>
    private static void EwmRejectsInfiniteAlpha()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        Action operation = () => table
            .Ewm("Id",
                double.PositiveInfinity);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies ewm rejects zero minimum.
    /// </summary>
    private static void EwmRejectsZeroMinimum()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        Action operation = () => table
            .Ewm("Id",
                0.5,
                0);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies window rejects negative variance adjustment.
    /// </summary>
    private static void WindowRejectsNegativeVarianceAdjustment()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        Action operation = () => table
            .Rolling("Id",
                2)
            .Variance(-1);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies window rejects negative deviation adjustment.
    /// </summary>
    private static void WindowRejectsNegativeDeviationAdjustment()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        Action operation = () => table
            .Rolling("Id",
                2)
            .Std(-1);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies window rejects unknown aggregate.
    /// </summary>
    private static void WindowRejectsUnknownAggregate()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        Action operation = () => table
            .Rolling("Id",
                2)
            .Aggregate((AggregateFunction)999);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies window rejects nonnumeric source.
    /// </summary>
    private static void WindowRejectsNonnumericSource()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Rolling("Key",
                1);

        // Assert
        Throws<ArgumentException>(operation);
    }

    #endregion
}
