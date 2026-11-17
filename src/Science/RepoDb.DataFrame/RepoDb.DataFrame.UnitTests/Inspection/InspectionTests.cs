using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Runs inspection tests without discovery attributes.
/// </summary>
internal static class InspectionTests
{
    #region Public Methods

    /// <summary>
    /// Runs all inspection scenarios.
    /// </summary>
    public static void Run()
    {
        RunSelectionTests();
        RunMetadataTests();
        RunStatisticsTests();
        RunDistinctTests();
        RunValidationTests();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs row selection scenarios.
    /// </summary>
    private static void RunSelectionTests()
    {
        HeadReturnsFirstRows();
        TailReturnsLastRows();
        SelectionHandlesBoundaries();
        SelectionPreservesSchema();
        SelectionDoesNotMutateSource();
        SelectionPreservesRowStates();
        SelectionPreservesExpressions();
        SampleIsReproducible();
        SampleHasNoReplacement();
        SampleHandlesBoundaries();
        SampleDoesNotMutateSource();
        EmptySelectionsPreserveColumns();
    }

    /// <summary>
    /// Runs metadata and memory scenarios.
    /// </summary>
    private static void RunMetadataTests()
    {
        ShapeReturnsDimensions();
        DeletedRowsAreExcluded();
        InfoDescribesTable();
        InfoDescribesColumns();
        InfoIsASnapshot();
        DTypesReturnsDeclaredTypes();
        EmptyTablesAreSupported();
        MemoryUsageCountsPayload();
        MemoryUsageHandlesSupportedTypes();
        MemoryUsageHandlesUnknownTypes();
        ModelsExposeConstructorValues();
        ModelsCopyColumnCollections();
    }

    /// <summary>
    /// Runs numeric statistics scenarios.
    /// </summary>
    private static void RunStatisticsTests()
    {
        DescribeReturnsNumericColumns();
        DescribeComputesMeanAndDeviation();
        DescribeComputesQuantiles();
        DescribeHandlesEmptyColumns();
        DescribeHandlesOneValue();
        DescribeHandlesDecimalAndMixedTypes();
        DescribeHandlesLargeOffsets();
        DescribeRejectsNonfiniteValues();
        DescribeSkipsMissingValues();
        DescribeHandlesConstantValues();
        DescribeHandlesUnsignedValues();
    }

    /// <summary>
    /// Runs distinct value scenarios.
    /// </summary>
    private static void RunDistinctTests()
    {
        UniquePreservesFirstSeenOrder();
        UniqueHandlesNulls();
        NUniqueHandlesNulls();
        ValueCountsReturnsFrequencies();
        ValueCountsHandlesNulls();
        DistinctUsesClrEquality();
        DistinctHandlesEmptyColumns();
        DistinctHandlesMixedTypes();
    }

    /// <summary>
    /// Runs argument validation scenarios.
    /// </summary>
    private static void RunValidationTests()
    {
        NullSelectionTablesAreRejected();
        NullMetadataTablesAreRejected();
        NullStatisticsTablesAreRejected();
        NullDistinctTablesAreRejected();
        InvalidColumnsAreRejected();
    }

    /// <summary>
    /// Checks the first row selection.
    /// </summary>
    private static void HeadReturnsFirstRows()
    {
        // Setup
        var table = CreateTable();

        // Act
        var result = table
            .Head(2);

        // Assert
        Check(result.Rows.Count == 2 && (int)result.Rows[0][0] == 1);
    }

    /// <summary>
    /// Checks the last row selection order.
    /// </summary>
    private static void TailReturnsLastRows()
    {
        // Setup
        var table = CreateTable();

        // Act
        var result = table
            .Tail(2);

        // Assert
        Check((int)result.Rows[0][0] == 3 && (int)result.Rows[1][0] == 4);
    }

    /// <summary>
    /// Checks row selection limits.
    /// </summary>
    private static void SelectionHandlesBoundaries()
    {
        // Setup
        var table = CreateTable();

        // Act
        var head = table
            .Head(0);
        var tail = table
            .Tail(99);
        Action negativeHead = () => table
            .Head(-1);
        Action negativeTail = () => table
            .Tail(-1);

        // Assert
        Check(head.Rows.Count == 0 && tail.Rows.Count == 4);
        Throws<ArgumentOutOfRangeException>(negativeHead);
        Throws<ArgumentOutOfRangeException>(negativeTail);
    }

    /// <summary>
    /// Checks copied schema metadata.
    /// </summary>
    private static void SelectionPreservesSchema()
    {
        // Setup
        var table = CreateTable();
        table.Columns[0].Caption = "Identifier";
        table.PrimaryKey = new[] { table.Columns[0] };

        // Act
        var result = table
            .Head(0);

        // Assert
        Check(result.PrimaryKey.Length == 1 && result.Columns[0].Caption is "Identifier");
    }

    /// <summary>
    /// Checks value isolation for scalar cells.
    /// </summary>
    private static void SelectionDoesNotMutateSource()
    {
        // Setup
        var table = CreateTable();

        // Act
        var result = table
            .Head();
        result.Rows[0][0] = 99;

        // Assert
        Check(!ReferenceEquals(table,
            result) && (int)table.Rows[0][0] == 1);
    }

    /// <summary>
    /// Checks imported original and current versions.
    /// </summary>
    private static void SelectionPreservesRowStates()
    {
        // Setup
        var table = CreateTable();
        table.AcceptChanges();
        table.Rows[0][0] = 10;

        // Act
        var row = table
            .Head(1).Rows[0];

        // Assert
        Check(row.RowState == DataRowState.Modified && (int)row[0, DataRowVersion.Original] == 1);
    }

    /// <summary>
    /// Checks local computed columns.
    /// </summary>
    [UnconditionalSuppressMessage("Trimming",
        "IL2026",
        Justification = "The constant expression uses only built-in integer arithmetic without reflected members.")]
    private static void SelectionPreservesExpressions()
    {
        // Setup
        var table = CreateTable();
        table.Columns.Add("Double",
            typeof(int),
            "Id * 2");

        // Act
        var result = table
            .Head(1);

        // Assert
        Check(result.Columns["Double"]!.Expression is "Id * 2" && (int)result.Rows[0]["Double"] == 2);
    }

    /// <summary>
    /// Checks seeded sample order.
    /// </summary>
    private static void SampleIsReproducible()
    {
        // Setup
        var table = CreateTable();

        // Act
        var first = table
            .Sample(3,
            42).Rows
                .Cast<DataRow>()
                .Select(row => row[0]);
        var second = table
            .Sample(3,
            42).Rows
                .Cast<DataRow>()
                .Select(row => row[0]);

        // Assert
        Check(first
            .SequenceEqual(second));
    }

    /// <summary>
    /// Checks unique sampled row positions.
    /// </summary>
    private static void SampleHasNoReplacement()
    {
        // Setup
        var table = CreateTable();

        // Act
        var result = table
            .Sample(4,
            7);

        // Assert
        Check(result
            .NUnique("Id") == 4);
    }

    /// <summary>
    /// Checks sample limits.
    /// </summary>
    private static void SampleHandlesBoundaries()
    {
        // Setup
        var table = CreateTable();

        // Act
        var result = table
            .Sample(0);
        Action negativeCount = () => table
            .Sample(-1);
        Action oversizedCount = () => table
            .Sample(5);

        // Assert
        Check(result.Rows.Count == 0);
        Throws<ArgumentOutOfRangeException>(negativeCount);
        Throws<ArgumentOutOfRangeException>(oversizedCount);
    }

    /// <summary>
    /// Checks sample isolation and source ordering.
    /// </summary>
    private static void SampleDoesNotMutateSource()
    {
        // Setup
        var table = CreateTable();

        // Act
        var result = table
            .Sample(4,
            42);
        result.Rows[0][0] = 99;

        // Assert
        Check(table.Rows
            .Cast<DataRow>()
            .Select(row => (int)row[0])
            .SequenceEqual(new[] { 1, 2, 3, 4 }));
    }

    /// <summary>
    /// Checks empty selections retain schema.
    /// </summary>
    private static void EmptySelectionsPreserveColumns()
    {
        // Setup
        var table = CreateTable()
            .Head(0);

        // Act
        var tail = table
            .Tail();
        var sample = table
            .Sample(0);

        // Assert
        Check(tail.Columns.Count == 2 && sample.Columns.Count == 2);
    }

    /// <summary>
    /// Checks table dimensions.
    /// </summary>
    private static void ShapeReturnsDimensions()
    {
        // Setup
        var table = CreateTable();

        // Act
        var result = table
            .Shape();

        // Assert
        Check(result == (4, 2));
    }

    /// <summary>
    /// Checks deleted rows across inspection operations.
    /// </summary>
    private static void DeletedRowsAreExcluded()
    {
        // Setup
        var table = CreateTable();
        table.AcceptChanges();
        table.Rows[0].Delete();

        // Act
        var shape = table
            .Shape();
        var head = table
            .Head();
        var info = table
            .Info();
        var statistics = table
            .Describe();
        var distinct = table
            .NUnique("Id");
        var memory = table
            .MemoryUsage();

        // Assert
        Check(shape.Rows == 3 && head.Rows.Count == 3);
        Check(info.RowCount == 3 && statistics[0].Count == 3);
        Check(distinct == 3 && memory == 16);
    }

    /// <summary>
    /// Checks table metadata counts.
    /// </summary>
    private static void InfoDescribesTable()
    {
        // Setup
        var table = CreateTable();

        // Act
        var info = table
            .Info();

        // Assert
        Check(info.RowCount == 4 && info.Columns.Count == 2);
    }

    /// <summary>
    /// Checks all column metadata properties.
    /// </summary>
    private static void InfoDescribesColumns()
    {
        // Setup
        var table = CreateTable();

        // Act
        var column = table
            .Info().Columns[1];

        // Assert
        Check(column.Name is "Category" && column.DataType == typeof(string) && column.AllowDBNull);
        Check(column.NullCount == 1 && column.NonNullCount == 3);
    }

    /// <summary>
    /// Checks metadata remains unchanged after source mutation.
    /// </summary>
    private static void InfoIsASnapshot()
    {
        // Setup
        var table = CreateTable();

        // Act
        var info = table
            .Info();
        table.Clear();

        // Assert
        Check(info.RowCount == 4 && info.Columns[1].NonNullCount == 3);
    }

    /// <summary>
    /// Checks declared type mapping.
    /// </summary>
    private static void DTypesReturnsDeclaredTypes()
    {
        // Setup
        var table = CreateTable();

        // Act
        var types = table
            .DTypes();

        // Assert
        Check(types["Id"] == typeof(int) && types["Category"] == typeof(string));
        Throws<NotSupportedException>(() => ((IDictionary<string, Type>)types).Clear());
    }

    /// <summary>
    /// Checks numeric column selection and names.
    /// </summary>
    private static void DescribeReturnsNumericColumns()
    {
        // Setup
        var table = CreateTable();

        // Act
        var result = table
            .Describe();

        // Assert
        Check(result.Count == 1 && result[0].Name is "Id" && result[0].Count == 4);
    }

    /// <summary>
    /// Checks mean and sample deviation.
    /// </summary>
    private static void DescribeComputesMeanAndDeviation()
    {
        // Setup
        var table = CreateTable();

        // Act
        var result = table
            .Describe()[0];

        // Assert
        Check(result.Mean == 2.5);
        Near(Math.Sqrt(5.0 / 3),
            result.StandardDeviation);
    }

    /// <summary>
    /// Checks linearly interpolated quantiles and extrema.
    /// </summary>
    private static void DescribeComputesQuantiles()
    {
        // Setup
        var table = CreateTable();

        // Act
        var result = table
            .Describe()[0];

        // Assert
        Check(result.Min == 1 && result.Max == 4);
        Check(result.FirstQuartile == 1.75 && result.Median == 2.5 && result.ThirdQuartile == 3.25);
    }

    /// <summary>
    /// Checks null statistics for empty and all-null columns.
    /// </summary>
    private static void DescribeHandlesEmptyColumns()
    {
        // Setup
        var table = CreateTable()
            .Head(0);
        table.Rows.Add(DBNull.Value,
            DBNull.Value);

        // Act
        var result = table
            .Describe()[0];

        // Assert
        Check(result.Count == 0 && result.Mean is null && result.StandardDeviation is null);
        Check(result.Min is null && result.Max is null && result.Median is null);
    }

    /// <summary>
    /// Checks singleton statistics.
    /// </summary>
    private static void DescribeHandlesOneValue()
    {
        // Setup
        var table = CreateTable();

        // Act
        var result = table
            .Head(1)
            .Describe()[0];

        // Assert
        Check(result.Mean == 1 && result.Median == 1 && result.StandardDeviation is null);
    }

    /// <summary>
    /// Checks numeric conversion and nonnumeric exclusion.
    /// </summary>
    private static void DescribeHandlesDecimalAndMixedTypes()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Amount",
            typeof(decimal));
        table.Columns.Add("Mixed",
            typeof(object));
        table.Rows.Add(1.5m,
            "text");
        table.Rows.Add(2.5m,
            5);

        // Act
        var result = table
            .Describe();

        // Assert
        Check(result.Count == 1 && result[0].Mean == 2);
    }

    /// <summary>
    /// Checks variance stability with large offsets.
    /// </summary>
    private static void DescribeHandlesLargeOffsets()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(double));
        table.Rows.Add(1e12 + 1);
        table.Rows.Add(1e12 + 2);
        table.Rows.Add(1e12 + 3);

        // Act
        var result = table
            .Describe()[0];

        // Assert
        Near(1,
            result.StandardDeviation);
    }

    /// <summary>
    /// Checks explicit rejection of nonfinite numbers.
    /// </summary>
    private static void DescribeRejectsNonfiniteValues()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(double));
        table.Rows.Add(double.NaN);
        var infinityTable = table.Copy();
        infinityTable.Rows[0][0] = double.PositiveInfinity;

        // Act
        Action describeNaN = () => table
            .Describe();
        Action describeInfinity = () => infinityTable
            .Describe();

        // Assert
        Throws<ArgumentException>(describeNaN);
        Throws<ArgumentException>(describeInfinity);
    }

    /// <summary>
    /// Checks missing numbers are skipped.
    /// </summary>
    private static void DescribeSkipsMissingValues()
    {
        // Setup
        var table = CreateTable();
        table.Rows[0][0] = DBNull.Value;

        // Act
        var result = table
            .Describe()[0];

        // Assert
        Check(result.Count == 3 && result.Mean == 3);
    }

    /// <summary>
    /// Checks repeated numbers have zero deviation.
    /// </summary>
    private static void DescribeHandlesConstantValues()
    {
        // Setup
        var table = CreateTable();
        foreach (DataRow row in table.Rows)
        {
            row[0] = 5;
        }

        // Act
        var result = table
            .Describe()[0];

        // Assert
        Check(result.StandardDeviation == 0);
    }

    /// <summary>
    /// Checks unsigned numeric columns.
    /// </summary>
    private static void DescribeHandlesUnsignedValues()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(ulong));
        table.Rows.Add(1ul);
        table.Rows.Add(3ul);

        // Act
        var result = table
            .Describe()[0];

        // Assert
        Check(result.Mean == 2);
    }

    /// <summary>
    /// Checks first-seen distinct order.
    /// </summary>
    private static void UniquePreservesFirstSeenOrder()
    {
        // Setup
        var table = CreateTable();

        // Act
        var result = table
            .Unique("Category",
            false);

        // Assert
        Check(result
            .SequenceEqual(new object?[] { "A", "B" }));
    }

    /// <summary>
    /// Checks normalized distinct nulls.
    /// </summary>
    private static void UniqueHandlesNulls()
    {
        // Setup
        var table = CreateTable();

        // Act
        var values = table
            .Unique("Category");

        // Assert
        Check(values.Count == 3 && values[2] is null);
    }

    /// <summary>
    /// Checks distinct null inclusion defaults.
    /// </summary>
    private static void NUniqueHandlesNulls()
    {
        // Setup
        var table = CreateTable();

        // Act
        var withoutNulls = table
            .NUnique("Category");
        var withNulls = table
            .NUnique("Category",
                true);

        // Assert
        Check(withoutNulls == 2 && withNulls == 3);
    }

    /// <summary>
    /// Checks frequency counts and ordering.
    /// </summary>
    private static void ValueCountsReturnsFrequencies()
    {
        // Setup
        var table = CreateTable();

        // Act
        var counts = table
            .ValueCounts("Category");

        // Assert
        Check(counts[0].Value is "A" && counts[0].Count == 2);
        Check(counts[1].Value is "B" && counts[1].Count == 1);
    }

    /// <summary>
    /// Checks null frequency inclusion.
    /// </summary>
    private static void ValueCountsHandlesNulls()
    {
        // Setup
        var table = CreateTable();

        // Act
        var counts = table
            .ValueCounts("Category",
            true);

        // Assert
        Check(counts[2].Value is null && counts[2].Count == 1);
    }

    /// <summary>
    /// Checks case-sensitive CLR value equality.
    /// </summary>
    private static void DistinctUsesClrEquality()
    {
        // Setup
        var table = CreateTable();
        table.Rows.Add(5,
            "a");

        // Act
        var result = table
            .NUnique("Category");

        // Assert
        Check(result == 3);
    }

    /// <summary>
    /// Checks distinct operations on empty columns.
    /// </summary>
    private static void DistinctHandlesEmptyColumns()
    {
        // Setup
        var table = CreateTable()
            .Head(0);

        // Act
        var unique = table
            .Unique("Id");
        var count = table
            .NUnique("Id");
        var frequencies = table
            .ValueCounts("Id");

        // Assert
        Check(unique.Count == 0 && count == 0);
        Check(frequencies.Count == 0);
    }

    /// <summary>
    /// Checks mixed CLR types remain distinct.
    /// </summary>
    private static void DistinctHandlesMixedTypes()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(object));
        table.Rows.Add(1);
        table.Rows.Add("1");
        table.Rows.Add(1L);

        // Act
        var result = table
            .NUnique("Value");

        // Assert
        Check(result == 3);
    }

    /// <summary>
    /// Checks empty table inspection.
    /// </summary>
    private static void EmptyTablesAreSupported()
    {
        // Setup
        var table = new DataTable();

        // Act
        var shape = table
            .Shape();
        var info = table
            .Info();
        var statistics = table
            .Describe();
        var memory = table
            .MemoryUsage();
        var head = table
            .Head();
        var tail = table
            .Tail();

        // Assert
        Check(shape == (0, 0) && info.Columns.Count == 0);
        Check(statistics.Count == 0 && memory == 0);
        Check(head.Rows.Count == 0 && tail.Rows.Count == 0);
    }

    /// <summary>
    /// Checks scalar payload sizes and missing values.
    /// </summary>
    private static void MemoryUsageCountsPayload()
    {
        // Setup
        var table = CreateTable();

        // Act
        var result = table
            .MemoryUsage();

        // Assert
        Check(result == 22);
    }

    /// <summary>
    /// Checks supported fixed and variable payload sizes.
    /// </summary>
    private static void MemoryUsageHandlesSupportedTypes()
    {
        // Setup
        var values = new object[] { true, (byte)1, (sbyte)1, 'a', (short)1, (ushort)1,
            1, 1u, 1f, 1L, 1ul, 1d, DateTime.MinValue, TimeSpan.Zero, 1m,
            Guid.Empty, DateTimeOffset.MinValue, new byte[3] };
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(object));
        foreach (var value in values)
        {
            table.Rows.Add(value);
        }

        // Act
        var result = table
            .MemoryUsage();

        // Assert
        Check(result == 112);
    }

    /// <summary>
    /// Checks fallback reference-size estimates.
    /// </summary>
    private static void MemoryUsageHandlesUnknownTypes()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(object));
        table.Rows.Add(new object());

        // Act
        var result = table
            .MemoryUsage();

        // Assert
        Check(result == IntPtr.Size);
    }

    /// <summary>
    /// Checks model constructor and property contracts.
    /// </summary>
    private static void ModelsExposeConstructorValues()
    {
        // Setup
        var name = "X";
        var type = typeof(int);

        // Act
        var column = new ColumnInfo(name,
            type,
            false,
            2,
            3);
        var value = new ValueCount("A",
            4);

        // Assert
        Check(column.Name is "X" && column.DataType == typeof(int) && !column.AllowDBNull);
        Check(column.NullCount == 2 && column.NonNullCount == 3);
        Check(value.Value is "A" && value.Count == 4);
    }

    /// <summary>
    /// Checks defensive copying of model collections.
    /// </summary>
    private static void ModelsCopyColumnCollections()
    {
        // Setup
        var columns = new List<ColumnInfo>();

        // Act
        var info = new TableInfo(7,
            columns);
        columns.Add(new ColumnInfo("X",
            typeof(int),
            true,
            0,
            7));

        // Assert
        Check(info.RowCount == 7 && info.Columns.Count == 0);
    }

    /// <summary>
    /// Checks null table guards for selection.
    /// </summary>
    private static void NullSelectionTablesAreRejected()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action head = () => table
            .Head();
        Action tail = () => table
            .Tail();
        Action sample = () => table
            .Sample();

        // Assert
        Throws<ArgumentNullException>(head);
        Throws<ArgumentNullException>(tail);
        Throws<ArgumentNullException>(sample);
    }

    /// <summary>
    /// Checks null table guards for metadata.
    /// </summary>
    private static void NullMetadataTablesAreRejected()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action shape = () => table
            .Shape();
        Action info = () => table
            .Info();
        Action types = () => table
            .DTypes();

        // Assert
        Throws<ArgumentNullException>(shape);
        Throws<ArgumentNullException>(info);
        Throws<ArgumentNullException>(types);
    }

    /// <summary>
    /// Checks null table guards for statistics.
    /// </summary>
    private static void NullStatisticsTablesAreRejected()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action describe = () => table
            .Describe();
        Action memory = () => table
            .MemoryUsage();

        // Assert
        Throws<ArgumentNullException>(describe);
        Throws<ArgumentNullException>(memory);
    }

    /// <summary>
    /// Checks null table guards for distinct values.
    /// </summary>
    private static void NullDistinctTablesAreRejected()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action unique = () => table
            .Unique("X");
        Action count = () => table
            .NUnique("X");
        Action frequencies = () => table
            .ValueCounts("X");

        // Assert
        Throws<ArgumentNullException>(unique);
        Throws<ArgumentNullException>(count);
        Throws<ArgumentNullException>(frequencies);
    }

    /// <summary>
    /// Checks invalid column guards.
    /// </summary>
    private static void InvalidColumnsAreRejected()
    {
        // Setup
        var table = CreateTable();

        // Act
        Action missingColumn = () => table
            .Unique("Missing");
        Action emptyColumn = () => table
            .NUnique("");
        Action nullColumn = () => table
            .ValueCounts(null!);

        // Assert
        Throws<ArgumentException>(missingColumn);
        Throws<ArgumentException>(emptyColumn);
        Throws<ArgumentNullException>(nullColumn);
    }

    /// <summary>
    /// Creates representative inspection data.
    /// </summary>
    /// <returns>The source table.</returns>
    private static DataTable CreateTable()
    {
        var table = new DataTable("Sample");
        table.Columns.Add("Id",
            typeof(int));
        table.Columns.Add("Category",
            typeof(string));
        table.Rows.Add(1,
            "A");
        table.Rows.Add(2,
            "B");
        table.Rows.Add(3,
            "A");
        table.Rows.Add(4,
            DBNull.Value);
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
            throw new InvalidOperationException("Inspection assertion failed.");
        }
    }

    /// <summary>
    /// Checks approximate numeric equality.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="actual">The actual value.</param>
    private static void Near(double expected,
        double? actual)
    {
        Check(actual.HasValue && Math.Abs(expected - actual.Value) < 1e-9);
    }

    /// <summary>
    /// Checks an expected exception.
    /// </summary>
    /// <typeparam name="TException">The exception type.</typeparam>
    /// <param name="action">The tested action.</param>
    private static void Throws<TException>(Action action) where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException)
        {
            return;
        }
        throw new InvalidOperationException("Expected exception was not thrown.");
    }

    #endregion
}
