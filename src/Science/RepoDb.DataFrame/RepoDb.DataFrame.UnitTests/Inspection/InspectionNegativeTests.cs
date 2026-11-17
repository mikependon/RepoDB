using System.Data;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests inspection failures and boundary behavior.
/// </summary>
internal static class InspectionNegativeTests
{
    #region Public Methods

    /// <summary>
    /// Runs negative and boundary scenarios.
    /// </summary>
    public static void Run()
    {
        RunColumnTests();
        RunSamplingTests();
        RunStatisticsTests();
        RunResultTests();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs column scenarios.
    /// </summary>
    private static void RunColumnTests()
    {
        UniqueRejectsNullColumn();
        UniqueRejectsEmptyColumn();
        UniqueRejectsWhitespaceColumn();
        UniqueRejectsMissingColumn();
        NUniqueRejectsNullColumn();
        NUniqueRejectsEmptyColumn();
        NUniqueRejectsWhitespaceColumn();
        NUniqueRejectsMissingColumn();
        ValueCountsRejectsNullColumn();
        ValueCountsRejectsEmptyColumn();
        ValueCountsRejectsWhitespaceColumn();
        ValueCountsRejectsMissingColumn();
        AmbiguousColumnNameIsRejected();
        ExactColumnNameResolvesAmbiguity();
    }

    /// <summary>
    /// Runs sampling scenarios.
    /// </summary>
    private static void RunSamplingTests()
    {
        HeadRejectsMinimumCount();
        TailRejectsMinimumCount();
        SampleRejectsMinimumCount();
        SampleRejectsMaximumCount();
        DefaultSampleRejectsInsufficientRows();
        SampleRejectsPositiveCountOnEmptyTable();
        UnseededSampleReturnsRequestedRows();
        MaximumSelectionLimitsReturnAvailableRows();
        SampleCountsCurrentRowsOnly();
        ZeroSampleRetainsSchemaOnDeletedOnlyTable();
    }

    /// <summary>
    /// Runs statistics scenarios.
    /// </summary>
    private static void RunStatisticsTests()
    {
        DeletedOnlyTableHasEmptyStatistics();
        DescribeRejectsNegativeInfinityWithoutMutation();
        DescribeRejectsSinglePrecisionNaN();
        DescribeExcludesNonnumericTypes();
        EmptyStatisticsHaveNoQuartiles();
        DescribeOrdersUnsortedNegativeNumbers();
    }

    /// <summary>
    /// Runs result scenarios.
    /// </summary>
    private static void RunResultTests()
    {
        DefaultUniqueIncludesOneMissingValue();
        UniqueResultsAreReadOnly();
        FrequencyResultsAreReadOnly();
        StatisticsResultsAreReadOnly();
        InfoColumnsAreReadOnly();
        TypeMappingIsAnIndependentSnapshot();
        StatisticsAreAnIndependentSnapshot();
        FrequencyResultsAreAnIndependentSnapshot();
    }

    /// <summary>
    /// Tests UniqueRejectsNullColumn.
    /// </summary>
    private static void UniqueRejectsNullColumn()
    {
        // Setup
        var table = CreateTable();

        // Act
        Action operation = () => table
            .Unique(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
        Check(table.Rows.Count == 2 && table.Columns.Count == 1);
    }

    /// <summary>
    /// Tests UniqueRejectsEmptyColumn.
    /// </summary>
    private static void UniqueRejectsEmptyColumn()
    {
        // Setup
        var table = CreateTable();

        // Act
        Action operation = () => table
            .Unique("");

        // Assert
        Throws<ArgumentException>(operation);
        Check(table.Rows.Count == 2 && table.Columns.Count == 1);
    }

    /// <summary>
    /// Tests UniqueRejectsWhitespaceColumn.
    /// </summary>
    private static void UniqueRejectsWhitespaceColumn()
    {
        // Setup
        var table = CreateTable();

        // Act
        Action operation = () => table
            .Unique(" \t ");

        // Assert
        Throws<ArgumentException>(operation);
        Check(table.Rows.Count == 2 && table.Columns.Count == 1);
    }

    /// <summary>
    /// Tests UniqueRejectsMissingColumn.
    /// </summary>
    private static void UniqueRejectsMissingColumn()
    {
        // Setup
        var table = CreateTable();

        // Act
        Action operation = () => table
            .Unique("Missing");

        // Assert
        Throws<ArgumentException>(operation);
        Check(table.Rows.Count == 2 && table.Columns.Count == 1);
    }

    /// <summary>
    /// Tests NUniqueRejectsNullColumn.
    /// </summary>
    private static void NUniqueRejectsNullColumn()
    {
        // Setup
        var table = CreateTable();

        // Act
        Action operation = () => table
            .NUnique(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
        Check(table.Rows.Count == 2 && table.Columns.Count == 1);
    }

    /// <summary>
    /// Tests NUniqueRejectsEmptyColumn.
    /// </summary>
    private static void NUniqueRejectsEmptyColumn()
    {
        // Setup
        var table = CreateTable();

        // Act
        Action operation = () => table
            .NUnique("");

        // Assert
        Throws<ArgumentException>(operation);
        Check(table.Rows.Count == 2 && table.Columns.Count == 1);
    }

    /// <summary>
    /// Tests NUniqueRejectsWhitespaceColumn.
    /// </summary>
    private static void NUniqueRejectsWhitespaceColumn()
    {
        // Setup
        var table = CreateTable();

        // Act
        Action operation = () => table
            .NUnique(" \t ");

        // Assert
        Throws<ArgumentException>(operation);
        Check(table.Rows.Count == 2 && table.Columns.Count == 1);
    }

    /// <summary>
    /// Tests NUniqueRejectsMissingColumn.
    /// </summary>
    private static void NUniqueRejectsMissingColumn()
    {
        // Setup
        var table = CreateTable();

        // Act
        Action operation = () => table
            .NUnique("Missing");

        // Assert
        Throws<ArgumentException>(operation);
        Check(table.Rows.Count == 2 && table.Columns.Count == 1);
    }

    /// <summary>
    /// Tests ValueCountsRejectsNullColumn.
    /// </summary>
    private static void ValueCountsRejectsNullColumn()
    {
        // Setup
        var table = CreateTable();

        // Act
        Action operation = () => table
            .ValueCounts(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
        Check(table.Rows.Count == 2 && table.Columns.Count == 1);
    }

    /// <summary>
    /// Tests ValueCountsRejectsEmptyColumn.
    /// </summary>
    private static void ValueCountsRejectsEmptyColumn()
    {
        // Setup
        var table = CreateTable();

        // Act
        Action operation = () => table
            .ValueCounts("");

        // Assert
        Throws<ArgumentException>(operation);
        Check(table.Rows.Count == 2 && table.Columns.Count == 1);
    }

    /// <summary>
    /// Tests ValueCountsRejectsWhitespaceColumn.
    /// </summary>
    private static void ValueCountsRejectsWhitespaceColumn()
    {
        // Setup
        var table = CreateTable();

        // Act
        Action operation = () => table
            .ValueCounts(" \t ");

        // Assert
        Throws<ArgumentException>(operation);
        Check(table.Rows.Count == 2 && table.Columns.Count == 1);
    }

    /// <summary>
    /// Tests ValueCountsRejectsMissingColumn.
    /// </summary>
    private static void ValueCountsRejectsMissingColumn()
    {
        // Setup
        var table = CreateTable();

        // Act
        Action operation = () => table
            .ValueCounts("Missing");

        // Assert
        Throws<ArgumentException>(operation);
        Check(table.Rows.Count == 2 && table.Columns.Count == 1);
    }

    /// <summary>
    /// Tests AmbiguousColumnNameIsRejected.
    /// </summary>
    private static void AmbiguousColumnNameIsRejected()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("ID",
            typeof(int));
        table.Columns.Add("id",
            typeof(int));

        // Act
        Action operation = () => table
            .Unique("Id");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Tests ExactColumnNameResolvesAmbiguity.
    /// </summary>
    private static void ExactColumnNameResolvesAmbiguity()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("ID",
            typeof(int));
        table.Columns.Add("id",
            typeof(int));
        table.Rows.Add(1,
            2);

        // Act
        var result = table
            .Unique("id");

        // Assert
        Check(result.Count == 1 && result[0] is 2);
    }

    /// <summary>
    /// Tests HeadRejectsMinimumCount.
    /// </summary>
    private static void HeadRejectsMinimumCount()
    {
        // Setup
        var table = CreateTable();

        // Act
        Action operation = () => table
            .Head(int.MinValue);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
        Check(table.Rows.Count == 2);
    }

    /// <summary>
    /// Tests TailRejectsMinimumCount.
    /// </summary>
    private static void TailRejectsMinimumCount()
    {
        // Setup
        var table = CreateTable();

        // Act
        Action operation = () => table
            .Tail(int.MinValue);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
        Check(table.Rows.Count == 2);
    }

    /// <summary>
    /// Tests SampleRejectsMinimumCount.
    /// </summary>
    private static void SampleRejectsMinimumCount()
    {
        // Setup
        var table = CreateTable();

        // Act
        Action operation = () => table
            .Sample(int.MinValue);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
        Check(table.Rows.Count == 2);
    }

    /// <summary>
    /// Tests SampleRejectsMaximumCount.
    /// </summary>
    private static void SampleRejectsMaximumCount()
    {
        // Setup
        var table = CreateTable();

        // Act
        Action operation = () => table
            .Sample(int.MaxValue);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
        Check(table.Rows.Count == 2);
    }

    /// <summary>
    /// Tests DefaultSampleRejectsInsufficientRows.
    /// </summary>
    private static void DefaultSampleRejectsInsufficientRows()
    {
        // Setup
        var table = CreateTable();

        // Act
        Action operation = () => table
            .Sample();

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Tests SampleRejectsPositiveCountOnEmptyTable.
    /// </summary>
    private static void SampleRejectsPositiveCountOnEmptyTable()
    {
        // Setup
        var table = CreateTable()
            .Clone();

        // Act
        Action operation = () => table
            .Sample(1);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Tests UnseededSampleReturnsRequestedRows.
    /// </summary>
    private static void UnseededSampleReturnsRequestedRows()
    {
        // Setup
        var table = CreateTable();

        // Act
        var result = table
            .Sample(1);

        // Assert
        Check(result.Rows.Count == 1 && table.Rows.Count == 2);
        Check(result.Rows[0][0] is 1 or 2);
    }

    /// <summary>
    /// Tests MaximumSelectionLimitsReturnAvailableRows.
    /// </summary>
    private static void MaximumSelectionLimitsReturnAvailableRows()
    {
        // Setup
        var table = CreateTable();

        // Act
        var head = table
            .Head(int.MaxValue);
        var tail = table
            .Tail(int.MaxValue);

        // Assert
        Check(head.Rows.Count == 2 && tail.Rows.Count == 2);
    }

    /// <summary>
    /// Tests SampleCountsCurrentRowsOnly.
    /// </summary>
    private static void SampleCountsCurrentRowsOnly()
    {
        // Setup
        var table = CreateTable();
        table.AcceptChanges();
        table.Rows[0].Delete();

        // Act
        Action operation = () => table
            .Sample(2);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
        Check(table.Rows[0].RowState == DataRowState.Deleted);
    }

    /// <summary>
    /// Tests DeletedOnlyTableHasEmptyStatistics.
    /// </summary>
    private static void DeletedOnlyTableHasEmptyStatistics()
    {
        // Setup
        var table = CreateTable();
        table.AcceptChanges();
        foreach (DataRow row in table.Rows)
        {
            row.Delete();
        }

        // Act
        var statistics = table
            .Describe()[0];
        var info = table
            .Info();

        // Assert
        Check(statistics.Count == 0 && statistics.Mean is null);
        Check(info.RowCount == 0 && info.Columns[0].NullCount == 0);
    }

    /// <summary>
    /// Tests DescribeRejectsNegativeInfinityWithoutMutation.
    /// </summary>
    private static void DescribeRejectsNegativeInfinityWithoutMutation()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(double));
        table.Rows.Add(double.NegativeInfinity);

        // Act
        Action operation = () => table
            .Describe();

        // Assert
        Throws<ArgumentException>(operation);
        Check((double)table.Rows[0][0] == double.NegativeInfinity);
    }

    /// <summary>
    /// Tests DescribeRejectsSinglePrecisionNaN.
    /// </summary>
    private static void DescribeRejectsSinglePrecisionNaN()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(float));
        table.Rows.Add(float.NaN);

        // Act
        Action operation = () => table
            .Describe();

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Tests DescribeExcludesNonnumericTypes.
    /// </summary>
    private static void DescribeExcludesNonnumericTypes()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Flag",
            typeof(bool));
        table.Columns.Add("Date",
            typeof(DateTime));
        table.Columns.Add("Enum",
            typeof(DayOfWeek));
        table.Rows.Add(true,
            DateTime.MinValue,
            DayOfWeek.Monday);

        // Act
        var result = table
            .Describe();

        // Assert
        Check(result.Count == 0);
    }

    /// <summary>
    /// Tests EmptyStatisticsHaveNoQuartiles.
    /// </summary>
    private static void EmptyStatisticsHaveNoQuartiles()
    {
        // Setup
        var table = CreateTable()
            .Clone();

        // Act
        var result = table
            .Describe()[0];

        // Assert
        Check(result.FirstQuartile is null && result.Median is null && result.ThirdQuartile is null);
    }

    /// <summary>
    /// Tests DescribeOrdersUnsortedNegativeNumbers.
    /// </summary>
    private static void DescribeOrdersUnsortedNegativeNumbers()
    {
        // Setup
        var table = CreateTable();
        table.Rows[0][0] = -1;
        table.Rows[1][0] = -5;

        // Act
        var result = table
            .Describe()[0];

        // Assert
        Check(result.Min == -5 && result.Max == -1 && result.Mean == -3);
        Check(result.FirstQuartile == -4 && result.Median == -3 && result.ThirdQuartile == -2);
    }

    /// <summary>
    /// Tests DefaultUniqueIncludesOneMissingValue.
    /// </summary>
    private static void DefaultUniqueIncludesOneMissingValue()
    {
        // Setup
        var table = CreateTable();
        table.Rows[0][0] = DBNull.Value;
        table.Rows[1][0] = DBNull.Value;

        // Act
        var result = table
            .Unique("Value");
        var count = table
            .NUnique("Value");
        var frequencies = table
            .ValueCounts("Value",
                true);

        // Assert
        Check(result.Count == 1 && result[0] is null && count == 0);
        Check(frequencies.Count == 1 && frequencies[0].Value is null && frequencies[0].Count == 2);
    }

    /// <summary>
    /// Tests UniqueResultsAreReadOnly.
    /// </summary>
    private static void UniqueResultsAreReadOnly()
    {
        // Setup
        var table = CreateTable();

        // Act
        var result = table
            .Unique("Value");
        Action operation = () => ((IList<object?>)result)
            .Clear();

        // Assert
        Throws<NotSupportedException>(operation);
        Check(result.Count == 2);
    }

    /// <summary>
    /// Tests FrequencyResultsAreReadOnly.
    /// </summary>
    private static void FrequencyResultsAreReadOnly()
    {
        // Setup
        var table = CreateTable();

        // Act
        var result = table
            .ValueCounts("Value");
        Action operation = () => ((IList<ValueCount>)result)
            .Clear();

        // Assert
        Throws<NotSupportedException>(operation);
        Check(result.Count == 2);
    }

    /// <summary>
    /// Tests StatisticsResultsAreReadOnly.
    /// </summary>
    private static void StatisticsResultsAreReadOnly()
    {
        // Setup
        var table = CreateTable();

        // Act
        var result = table
            .Describe();
        Action operation = () => ((IList<ColumnStatistics>)result)
            .Clear();

        // Assert
        Throws<NotSupportedException>(operation);
        Check(result.Count == 1);
    }

    /// <summary>
    /// Tests InfoColumnsAreReadOnly.
    /// </summary>
    private static void InfoColumnsAreReadOnly()
    {
        // Setup
        var table = CreateTable();

        // Act
        var result = table
            .Info();
        Action operation = () => ((IList<ColumnInfo>)result.Columns)
            .Clear();

        // Assert
        Throws<NotSupportedException>(operation);
        Check(result.Columns.Count == 1);
    }

    /// <summary>
    /// Tests TypeMappingIsAnIndependentSnapshot.
    /// </summary>
    private static void TypeMappingIsAnIndependentSnapshot()
    {
        // Setup
        var table = CreateTable();

        // Act
        var result = table
            .DTypes();
        table.Columns[0].ColumnName = "Changed";

        // Assert
        Check(result.ContainsKey("Value") && !result.ContainsKey("Changed"));
    }

    /// <summary>
    /// Tests StatisticsAreAnIndependentSnapshot.
    /// </summary>
    private static void StatisticsAreAnIndependentSnapshot()
    {
        // Setup
        var table = CreateTable();

        // Act
        var result = table
            .Describe()[0];
        table.Clear();

        // Assert
        Check(result.Count == 2 && result.Mean == 1.5);
    }

    /// <summary>
    /// Tests FrequencyResultsAreAnIndependentSnapshot.
    /// </summary>
    private static void FrequencyResultsAreAnIndependentSnapshot()
    {
        // Setup
        var table = CreateTable();

        // Act
        var result = table
            .ValueCounts("Value");
        table.Clear();

        // Assert
        Check(result.Count == 2 && result[0].Count == 1);
    }

    /// <summary>
    /// Tests ZeroSampleRetainsSchemaOnDeletedOnlyTable.
    /// </summary>
    private static void ZeroSampleRetainsSchemaOnDeletedOnlyTable()
    {
        // Setup
        var table = CreateTable();
        table.AcceptChanges();
        foreach (DataRow row in table.Rows)
        {
            row.Delete();
        }

        // Act
        var result = table
            .Sample(0,
                int.MinValue);

        // Assert
        Check(result.Rows.Count == 0 && result.Columns[0].DataType == typeof(int));
    }
    /// <summary>
    /// Creates a small numeric table.
    /// </summary>
    /// <returns>The test table.</returns>
    private static DataTable CreateTable()
    {
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(int));
        table.Rows.Add(1);
        table.Rows.Add(2);
        return table;
    }

    /// <summary>
    /// Checks a condition.
    /// </summary>
    /// <param name="condition">The expected condition.</param>
    private static void Check(bool condition)
    {
        if (!condition)
        {
            throw new InvalidOperationException("Negative inspection assertion failed.");
        }
    }

    /// <summary>
    /// Checks the exact exception type.
    /// </summary>
    /// <typeparam name="TException">The expected exception type.</typeparam>
    /// <param name="operation">The tested operation.</param>
    private static void Throws<TException>(Action operation) where TException : Exception
    {
        try
        {
            operation();
        }
        catch (TException exception)
        {
            Check(exception.GetType() == typeof(TException));
            return;
        }
        throw new InvalidOperationException("Expected exception was not thrown.");
    }

    #endregion
}
