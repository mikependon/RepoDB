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
        var result = CreateTable()
            .Head(2);
        Check(result.Rows.Count == 2 && (int)result.Rows[0][0] == 1);
    }

    /// <summary>
    /// Checks the last row selection order.
    /// </summary>
    private static void TailReturnsLastRows()
    {
        var result = CreateTable()
            .Tail(2);
        Check((int)result.Rows[0][0] == 3 && (int)result.Rows[1][0] == 4);
    }

    /// <summary>
    /// Checks row selection limits.
    /// </summary>
    private static void SelectionHandlesBoundaries()
    {
        var table = CreateTable();
        Check(table
            .Head(0).Rows.Count == 0 && table
            .Tail(99).Rows.Count == 4);
        Throws<ArgumentOutOfRangeException>(() => table
            .Head(-1));
        Throws<ArgumentOutOfRangeException>(() => table
            .Tail(-1));
    }

    /// <summary>
    /// Checks copied schema metadata.
    /// </summary>
    private static void SelectionPreservesSchema()
    {
        var table = CreateTable();
        table.Columns[0].Caption = "Identifier";
        table.PrimaryKey = new[] { table.Columns[0] };
        var result = table
            .Head(0);
        Check(result.PrimaryKey.Length == 1 && result.Columns[0].Caption is "Identifier");
    }

    /// <summary>
    /// Checks value isolation for scalar cells.
    /// </summary>
    private static void SelectionDoesNotMutateSource()
    {
        var table = CreateTable();
        var result = table
            .Head();
        result.Rows[0][0] = 99;
        Check(!ReferenceEquals(table,
            result) && (int)table.Rows[0][0] == 1);
    }

    /// <summary>
    /// Checks imported original and current versions.
    /// </summary>
    private static void SelectionPreservesRowStates()
    {
        var table = CreateTable();
        table.AcceptChanges();
        table.Rows[0][0] = 10;
        var row = table
            .Head(1).Rows[0];
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
        var table = CreateTable();
        table.Columns.Add("Double",
            typeof(int),
            "Id * 2");
        var result = table
            .Head(1);
        Check(result.Columns["Double"]!.Expression is "Id * 2" && (int)result.Rows[0]["Double"] == 2);
    }

    /// <summary>
    /// Checks seeded sample order.
    /// </summary>
    private static void SampleIsReproducible()
    {
        var table = CreateTable();
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
        Check(first
            .SequenceEqual(second));
    }

    /// <summary>
    /// Checks unique sampled row positions.
    /// </summary>
    private static void SampleHasNoReplacement()
    {
        var result = CreateTable()
            .Sample(4,
            7);
        Check(result
            .NUnique("Id") == 4);
    }

    /// <summary>
    /// Checks sample limits.
    /// </summary>
    private static void SampleHandlesBoundaries()
    {
        var table = CreateTable();
        Check(table
            .Sample(0).Rows.Count == 0);
        Throws<ArgumentOutOfRangeException>(() => table
            .Sample(-1));
        Throws<ArgumentOutOfRangeException>(() => table
            .Sample(5));
    }

    /// <summary>
    /// Checks sample isolation and source ordering.
    /// </summary>
    private static void SampleDoesNotMutateSource()
    {
        var table = CreateTable();
        var result = table
            .Sample(4,
            42);
        result.Rows[0][0] = 99;
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
        var table = CreateTable()
            .Head(0);
        Check(table
            .Tail().Columns.Count == 2 && table
            .Sample(0).Columns.Count == 2);
    }

    /// <summary>
    /// Checks table dimensions.
    /// </summary>
    private static void ShapeReturnsDimensions()
    {
        Check(CreateTable()
            .Shape() == (4,
            2));
    }

    /// <summary>
    /// Checks deleted rows across inspection operations.
    /// </summary>
    private static void DeletedRowsAreExcluded()
    {
        var table = CreateTable();
        table.AcceptChanges();
        table.Rows[0].Delete();
        Check(table
            .Shape().Rows == 3 && table
            .Head().Rows.Count == 3);
        Check(table
            .Info().RowCount == 3 && table
            .Describe()[0].Count == 3);
        Check(table
            .NUnique("Id") == 3 && table
            .MemoryUsage() == 16);
    }

    /// <summary>
    /// Checks table metadata counts.
    /// </summary>
    private static void InfoDescribesTable()
    {
        var info = CreateTable()
            .Info();
        Check(info.RowCount == 4 && info.Columns.Count == 2);
    }

    /// <summary>
    /// Checks all column metadata properties.
    /// </summary>
    private static void InfoDescribesColumns()
    {
        var column = CreateTable()
            .Info().Columns[1];
        Check(column.Name is "Category" && column.DataType == typeof(string) && column.AllowDBNull);
        Check(column.NullCount == 1 && column.NonNullCount == 3);
    }

    /// <summary>
    /// Checks metadata remains unchanged after source mutation.
    /// </summary>
    private static void InfoIsASnapshot()
    {
        var table = CreateTable();
        var info = table
            .Info();
        table.Clear();
        Check(info.RowCount == 4 && info.Columns[1].NonNullCount == 3);
    }

    /// <summary>
    /// Checks declared type mapping.
    /// </summary>
    private static void DTypesReturnsDeclaredTypes()
    {
        var types = CreateTable()
            .DTypes();
        Check(types["Id"] == typeof(int) && types["Category"] == typeof(string));
        Throws<NotSupportedException>(() => ((IDictionary<string, Type>)types).Clear());
    }

    /// <summary>
    /// Checks numeric column selection and names.
    /// </summary>
    private static void DescribeReturnsNumericColumns()
    {
        var result = CreateTable()
            .Describe();
        Check(result.Count == 1 && result[0].Name is "Id" && result[0].Count == 4);
    }

    /// <summary>
    /// Checks mean and sample deviation.
    /// </summary>
    private static void DescribeComputesMeanAndDeviation()
    {
        var result = CreateTable()
            .Describe()[0];
        Check(result.Mean == 2.5);
        Near(Math.Sqrt(5.0 / 3),
            result.StandardDeviation);
    }

    /// <summary>
    /// Checks linearly interpolated quantiles and extrema.
    /// </summary>
    private static void DescribeComputesQuantiles()
    {
        var result = CreateTable()
            .Describe()[0];
        Check(result.Min == 1 && result.Max == 4);
        Check(result.FirstQuartile == 1.75 && result.Median == 2.5 && result.ThirdQuartile == 3.25);
    }

    /// <summary>
    /// Checks null statistics for empty and all-null columns.
    /// </summary>
    private static void DescribeHandlesEmptyColumns()
    {
        var table = CreateTable()
            .Head(0);
        table.Rows.Add(DBNull.Value,
            DBNull.Value);
        var result = table
            .Describe()[0];
        Check(result.Count == 0 && result.Mean is null && result.StandardDeviation is null);
        Check(result.Min is null && result.Max is null && result.Median is null);
    }

    /// <summary>
    /// Checks singleton statistics.
    /// </summary>
    private static void DescribeHandlesOneValue()
    {
        var result = CreateTable()
            .Head(1)
            .Describe()[0];
        Check(result.Mean == 1 && result.Median == 1 && result.StandardDeviation is null);
    }

    /// <summary>
    /// Checks numeric conversion and nonnumeric exclusion.
    /// </summary>
    private static void DescribeHandlesDecimalAndMixedTypes()
    {
        var table = new DataTable();
        table.Columns.Add("Amount",
            typeof(decimal));
        table.Columns.Add("Mixed",
            typeof(object));
        table.Rows.Add(1.5m,
            "text");
        table.Rows.Add(2.5m,
            5);
        Check(table
            .Describe().Count == 1 && table
            .Describe()[0].Mean == 2);
    }

    /// <summary>
    /// Checks variance stability with large offsets.
    /// </summary>
    private static void DescribeHandlesLargeOffsets()
    {
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(double));
        table.Rows.Add(1e12 + 1);
        table.Rows.Add(1e12 + 2);
        table.Rows.Add(1e12 + 3);
        Near(1,
            table
                .Describe()[0].StandardDeviation);
    }

    /// <summary>
    /// Checks explicit rejection of nonfinite numbers.
    /// </summary>
    private static void DescribeRejectsNonfiniteValues()
    {
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(double));
        table.Rows.Add(double.NaN);
        Throws<ArgumentException>(() => table
            .Describe());
        table.Rows[0][0] = double.PositiveInfinity;
        Throws<ArgumentException>(() => table
            .Describe());
    }

    /// <summary>
    /// Checks missing numbers are skipped.
    /// </summary>
    private static void DescribeSkipsMissingValues()
    {
        var table = CreateTable();
        table.Rows[0][0] = DBNull.Value;
        var result = table
            .Describe()[0];
        Check(result.Count == 3 && result.Mean == 3);
    }

    /// <summary>
    /// Checks repeated numbers have zero deviation.
    /// </summary>
    private static void DescribeHandlesConstantValues()
    {
        var table = CreateTable();
        foreach (DataRow row in table.Rows)
        {
            row[0] = 5;
        }
        Check(table
            .Describe()[0].StandardDeviation == 0);
    }

    /// <summary>
    /// Checks unsigned numeric columns.
    /// </summary>
    private static void DescribeHandlesUnsignedValues()
    {
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(ulong));
        table.Rows.Add(1ul);
        table.Rows.Add(3ul);
        Check(table
            .Describe()[0].Mean == 2);
    }

    /// <summary>
    /// Checks first-seen distinct order.
    /// </summary>
    private static void UniquePreservesFirstSeenOrder()
    {
        var result = CreateTable()
            .Unique("Category",
            false);
        Check(result
            .SequenceEqual(new object?[] { "A", "B" }));
    }

    /// <summary>
    /// Checks normalized distinct nulls.
    /// </summary>
    private static void UniqueHandlesNulls()
    {
        var values = CreateTable()
            .Unique("Category");
        Check(values.Count == 3 && values[2] is null);
    }

    /// <summary>
    /// Checks distinct null inclusion defaults.
    /// </summary>
    private static void NUniqueHandlesNulls()
    {
        var table = CreateTable();
        Check(table
            .NUnique("Category") == 2 && table
            .NUnique("Category",
            true) == 3);
    }

    /// <summary>
    /// Checks frequency counts and ordering.
    /// </summary>
    private static void ValueCountsReturnsFrequencies()
    {
        var counts = CreateTable()
            .ValueCounts("Category");
        Check(counts[0].Value is "A" && counts[0].Count == 2);
        Check(counts[1].Value is "B" && counts[1].Count == 1);
    }

    /// <summary>
    /// Checks null frequency inclusion.
    /// </summary>
    private static void ValueCountsHandlesNulls()
    {
        var counts = CreateTable()
            .ValueCounts("Category",
            true);
        Check(counts[2].Value is null && counts[2].Count == 1);
    }

    /// <summary>
    /// Checks case-sensitive CLR value equality.
    /// </summary>
    private static void DistinctUsesClrEquality()
    {
        var table = CreateTable();
        table.Rows.Add(5,
            "a");
        Check(table
            .NUnique("Category") == 3);
    }

    /// <summary>
    /// Checks distinct operations on empty columns.
    /// </summary>
    private static void DistinctHandlesEmptyColumns()
    {
        var table = CreateTable()
            .Head(0);
        Check(table
            .Unique("Id").Count == 0 && table
            .NUnique("Id") == 0);
        Check(table
            .ValueCounts("Id").Count == 0);
    }

    /// <summary>
    /// Checks mixed CLR types remain distinct.
    /// </summary>
    private static void DistinctHandlesMixedTypes()
    {
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(object));
        table.Rows.Add(1);
        table.Rows.Add("1");
        table.Rows.Add(1L);
        Check(table
            .NUnique("Value") == 3);
    }

    /// <summary>
    /// Checks empty table inspection.
    /// </summary>
    private static void EmptyTablesAreSupported()
    {
        var table = new DataTable();
        Check(table
            .Shape() == (0,
            0) && table
                .Info().Columns.Count == 0);
        Check(table
            .Describe().Count == 0 && table
            .MemoryUsage() == 0);
        Check(table
            .Head().Rows.Count == 0 && table
            .Tail().Rows.Count == 0);
    }

    /// <summary>
    /// Checks scalar payload sizes and missing values.
    /// </summary>
    private static void MemoryUsageCountsPayload()
    {
        Check(CreateTable()
            .MemoryUsage() == 22);
    }

    /// <summary>
    /// Checks supported fixed and variable payload sizes.
    /// </summary>
    private static void MemoryUsageHandlesSupportedTypes()
    {
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
        Check(table
            .MemoryUsage() == 112);
    }

    /// <summary>
    /// Checks fallback reference-size estimates.
    /// </summary>
    private static void MemoryUsageHandlesUnknownTypes()
    {
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(object));
        table.Rows.Add(new object());
        Check(table
            .MemoryUsage() == IntPtr.Size);
    }

    /// <summary>
    /// Checks model constructor and property contracts.
    /// </summary>
    private static void ModelsExposeConstructorValues()
    {
        var column = new ColumnInfo("X",
            typeof(int),
            false,
            2,
            3);
        var value = new ValueCount("A",
            4);
        Check(column.Name is "X" && column.DataType == typeof(int) && !column.AllowDBNull);
        Check(column.NullCount == 2 && column.NonNullCount == 3);
        Check(value.Value is "A" && value.Count == 4);
    }

    /// <summary>
    /// Checks defensive copying of model collections.
    /// </summary>
    private static void ModelsCopyColumnCollections()
    {
        var columns = new List<ColumnInfo>();
        var info = new TableInfo(7,
            columns);
        columns.Add(new ColumnInfo("X",
            typeof(int),
            true,
            0,
            7));
        Check(info.RowCount == 7 && info.Columns.Count == 0);
    }

    /// <summary>
    /// Checks null table guards for selection.
    /// </summary>
    private static void NullSelectionTablesAreRejected()
    {
        DataTable table = null!;
        Throws<ArgumentNullException>(() => table
            .Head());
        Throws<ArgumentNullException>(() => table
            .Tail());
        Throws<ArgumentNullException>(() => table
            .Sample());
    }

    /// <summary>
    /// Checks null table guards for metadata.
    /// </summary>
    private static void NullMetadataTablesAreRejected()
    {
        DataTable table = null!;
        Throws<ArgumentNullException>(() => table
            .Shape());
        Throws<ArgumentNullException>(() => table
            .Info());
        Throws<ArgumentNullException>(() => table
            .DTypes());
    }

    /// <summary>
    /// Checks null table guards for statistics.
    /// </summary>
    private static void NullStatisticsTablesAreRejected()
    {
        DataTable table = null!;
        Throws<ArgumentNullException>(() => table
            .Describe());
        Throws<ArgumentNullException>(() => table
            .MemoryUsage());
    }

    /// <summary>
    /// Checks null table guards for distinct values.
    /// </summary>
    private static void NullDistinctTablesAreRejected()
    {
        DataTable table = null!;
        Throws<ArgumentNullException>(() => table
            .Unique("X"));
        Throws<ArgumentNullException>(() => table
            .NUnique("X"));
        Throws<ArgumentNullException>(() => table
            .ValueCounts("X"));
    }

    /// <summary>
    /// Checks invalid column guards.
    /// </summary>
    private static void InvalidColumnsAreRejected()
    {
        var table = CreateTable();
        Throws<ArgumentException>(() => table
            .Unique("Missing"));
        Throws<ArgumentException>(() => table
            .NUnique(""));
        Throws<ArgumentNullException>(() => table
            .ValueCounts(null!));
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
