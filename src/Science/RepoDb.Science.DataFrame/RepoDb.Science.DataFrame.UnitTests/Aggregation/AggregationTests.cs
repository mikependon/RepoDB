using System.Data;
using System.Diagnostics.CodeAnalysis;
using RepoDb.Science.UnitTests.Helpers;
using static RepoDb.Science.UnitTests.Helpers.AssertHelper;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Verifies aggregation operations and failure behavior.
/// </summary>
internal static class AggregationTests
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
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch1()
    {
        ScalarSumPreservesDecimalPrecision();
        ScalarMeanSkipsMissingValues();
        ScalarMinPreservesInputType();
        ScalarMaxPreservesInputType();
        TypedSummariesConvertResults();
        MissingScalarValuesReturnNull();
        GroupedAggregationPreservesOrderAndTypes();
        GroupedMinAndMaxPreserveTypes();
    }

    /// <summary>
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch2()
    {
        GlobalAggregationReturnsOneRow();
        GlobalEmptyAggregationReturnsMissingAndZeroCount();
        EmptyGroupedAggregationRetainsSchema();
        GroupingIncludesMissingCompositeKeys();
        GroupingReadsCurrentValuesAtExecution();
        CountAcceptsNonnumericColumn();
        AggregatesSkipDeletedRows();
        GroupByRejectsEmptyKeys();
    }

    /// <summary>
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch3()
    {
        GroupByRejectsMissingKeys();
        GroupByRejectsDuplicateKeys();
        AggregateRejectsNoDefinitions();
        AggregateRejectsNullArray();
        AggregateRejectsNullDefinition();
        AggregateRejectsUnknownColumn();
        AggregateRejectsDuplicateOutput();
        DefinitionRejectsUnknownFunction();
    }

    /// <summary>
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch4()
    {
        DefinitionRejectsEmptyName();
        DefinitionRejectsBlankAlias();
        TypedSummaryRejectsNonnumericResult();
        SumRejectsTextColumn();
        MeanRejectsTextColumn();
        MinRejectsMissingColumn();
        MaxRejectsTextColumn();
        SumRejectsOverflow();
    }

    /// <summary>
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch5()
    {
        TypedSummaryRejectsOverflow();
        MeanRejectsNonfiniteValues();
    }

    /// <summary>
    /// Verifies ScalarSumPreservesDecimalPrecision.
    /// </summary>
    private static void ScalarSumPreservesDecimalPrecision()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Sum("Value");

        // Assert
        Check(result == 70m);
    }

    /// <summary>
    /// Verifies ScalarMeanSkipsMissingValues.
    /// </summary>
    private static void ScalarMeanSkipsMissingValues()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Mean("Value");

        // Assert
        Check(result.HasValue && Math.Abs(result.Value - 70.0 / 3) < 1e-12);
    }

    /// <summary>
    /// Verifies ScalarMinPreservesInputType.
    /// </summary>
    private static void ScalarMinPreservesInputType()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Min("Count");

        // Assert
        Check(result is 1);
    }

    /// <summary>
    /// Verifies ScalarMaxPreservesInputType.
    /// </summary>
    private static void ScalarMaxPreservesInputType()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Max("Value");

        // Assert
        Check(result is decimal value && value == 30m);
    }

    /// <summary>
    /// Verifies TypedSummariesConvertResults.
    /// </summary>
    private static void TypedSummariesConvertResults()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var sum = table
            .Sum<long>("Count");
        var min = table
            .Min<int>("Count");
        var max = table
            .Max<double>("Value");

        // Assert
        Check(sum == 9 && min == 1 && max == 30);
    }

    /// <summary>
    /// Verifies MissingScalarValuesReturnNull.
    /// </summary>
    private static void MissingScalarValuesReturnNull()
    {
        // Setup
        var table = TestTable.Create()
            .Clone();

        // Act
        var sum = table
            .Sum("Value");
        var min = table
            .Min("Value");
        var max = table
            .Max("Value");
        var mean = table
            .Mean("Value");
        var typed = table
            .Sum<int>("Count");

        // Assert
        Check(sum is null && min is null && max is null);
        Check(mean is null && typed is null);
    }

    /// <summary>
    /// Verifies GroupedAggregationPreservesOrderAndTypes.
    /// </summary>
    private static void GroupedAggregationPreservesOrderAndTypes()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .GroupBy("Key")
            .Aggregate(new AggregateDefinition("Value",
                AggregateFunction.Sum,
                "Total"),
                new AggregateDefinition("Value",
                    AggregateFunction.Mean,
                    "Average"),
                new AggregateDefinition("Value",
                    AggregateFunction.Count,
                    "Present"));

        // Assert
        Check(result.Rows.Count == 2 && result.Rows[0]["Key"] is "A");
        Check((decimal)result.Rows[1]["Total"] == 60m && (int)result.Rows[0]["Present"] == 1);
    }

    /// <summary>
    /// Verifies GroupedMinAndMaxPreserveTypes.
    /// </summary>
    private static void GroupedMinAndMaxPreserveTypes()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .GroupBy("Key")
            .Aggregate(new AggregateDefinition("Count",
                AggregateFunction.Min,
                "Small"),
                new AggregateDefinition("Count",
                    AggregateFunction.Max,
                    "Large"));

        // Assert
        Check(result.Columns["Small"]!.DataType == typeof(int));
        Check((int)result.Rows[0]["Small"] == 1 && (int)result.Rows[0]["Large"] == 2);
    }

    /// <summary>
    /// Verifies GlobalAggregationReturnsOneRow.
    /// </summary>
    private static void GlobalAggregationReturnsOneRow()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Aggregate(new AggregateDefinition("Value",
                AggregateFunction.Sum));

        // Assert
        Check(result.Rows.Count == 1 && (decimal)result.Rows[0]["Value"] == 70m);
    }

    /// <summary>
    /// Verifies GlobalEmptyAggregationReturnsMissingAndZeroCount.
    /// </summary>
    private static void GlobalEmptyAggregationReturnsMissingAndZeroCount()
    {
        // Setup
        var table = TestTable.Create()
            .Clone();

        // Act
        var result = table
            .Aggregate(new AggregateDefinition("Value",
                AggregateFunction.Sum,
                "Sum"),
                new AggregateDefinition("Value",
                    AggregateFunction.Count,
                    "Count"));

        // Assert
        Check(result.Rows.Count == 1 && result.Rows[0].IsNull("Sum") && (int)result.Rows[0]["Count"] == 0);
    }

    /// <summary>
    /// Verifies EmptyGroupedAggregationRetainsSchema.
    /// </summary>
    private static void EmptyGroupedAggregationRetainsSchema()
    {
        // Setup
        var table = TestTable.Create()
            .Clone();

        // Act
        var result = table
            .GroupBy("Key")
            .Aggregate(new AggregateDefinition("Value",
                AggregateFunction.Sum));

        // Assert
        Check(result.Rows.Count == 0 && result.Columns.Count == 2);
    }

    /// <summary>
    /// Verifies GroupingIncludesMissingCompositeKeys.
    /// </summary>
    private static void GroupingIncludesMissingCompositeKeys()
    {
        // Setup
        var table = TestTable.Create();
        table.Rows[0]["Key"] = DBNull.Value;
        table.Rows[1]["Key"] = DBNull.Value;

        // Act
        var result = table
            .GroupBy("Key",
                "Count")
                .Aggregate(new AggregateDefinition("Value",
                    AggregateFunction.Count,
                    "Present"));

        // Assert
        Check(result.Rows.Count == 3 && result.Rows[0].IsNull("Key"));
    }

    /// <summary>
    /// Verifies GroupingReadsCurrentValuesAtExecution.
    /// </summary>
    private static void GroupingReadsCurrentValuesAtExecution()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var grouped = table
            .GroupBy("Key");
        table.Rows[0]["Value"] = 20m;
        var result = grouped
            .Aggregate(new AggregateDefinition("Value",
                AggregateFunction.Sum));

        // Assert
        Check((decimal)result.Rows[0]["Value"] == 20m);
    }

    /// <summary>
    /// Verifies CountAcceptsNonnumericColumn.
    /// </summary>
    private static void CountAcceptsNonnumericColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Aggregate(new AggregateDefinition("Key",
                AggregateFunction.Count,
                "Count"));

        // Assert
        Check((int)result.Rows[0][0] == 4);
    }

    /// <summary>
    /// Verifies AggregatesSkipDeletedRows.
    /// </summary>
    private static void AggregatesSkipDeletedRows()
    {
        // Setup
        var table = TestTable.Create();
        table.AcceptChanges();
        table.Rows[0].Delete();

        // Act
        var result = table
            .Sum("Value");

        // Assert
        Check(result == 60m);
    }

    /// <summary>
    /// Verifies GroupByRejectsEmptyKeys.
    /// </summary>
    private static void GroupByRejectsEmptyKeys()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .GroupBy();

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies GroupByRejectsMissingKeys.
    /// </summary>
    private static void GroupByRejectsMissingKeys()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .GroupBy("Missing");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies GroupByRejectsDuplicateKeys.
    /// </summary>
    private static void GroupByRejectsDuplicateKeys()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .GroupBy("Key",
                "Key");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies AggregateRejectsNoDefinitions.
    /// </summary>
    private static void AggregateRejectsNoDefinitions()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Aggregate();

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies AggregateRejectsNullArray.
    /// </summary>
    private static void AggregateRejectsNullArray()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Aggregate(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies AggregateRejectsNullDefinition.
    /// </summary>
    private static void AggregateRejectsNullDefinition()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Aggregate(new AggregateDefinition[] { null! });

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies AggregateRejectsUnknownColumn.
    /// </summary>
    private static void AggregateRejectsUnknownColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Aggregate(new AggregateDefinition("Missing",
                AggregateFunction.Sum));

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies AggregateRejectsDuplicateOutput.
    /// </summary>
    private static void AggregateRejectsDuplicateOutput()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .GroupBy("Key")
            .Aggregate(new AggregateDefinition("Value",
                AggregateFunction.Sum,
                "Key"));

        // Assert
        Throws<DuplicateNameException>(operation);
    }

    /// <summary>
    /// Verifies DefinitionRejectsUnknownFunction.
    /// </summary>
    private static void DefinitionRejectsUnknownFunction()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => new AggregateDefinition("Value",
            (AggregateFunction)999);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies DefinitionRejectsEmptyName.
    /// </summary>
    private static void DefinitionRejectsEmptyName()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => new AggregateDefinition("",
            AggregateFunction.Sum);

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies DefinitionRejectsBlankAlias.
    /// </summary>
    private static void DefinitionRejectsBlankAlias()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => new AggregateDefinition("Value",
            AggregateFunction.Sum,
            " ");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies TypedSummaryRejectsNonnumericResult.
    /// </summary>
    private static void TypedSummaryRejectsNonnumericResult()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Sum<Guid>("Value");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies SumRejectsTextColumn.
    /// </summary>
    private static void SumRejectsTextColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Sum("Key");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies MeanRejectsTextColumn.
    /// </summary>
    private static void MeanRejectsTextColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Mean("Key");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies MinRejectsMissingColumn.
    /// </summary>
    private static void MinRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Min("Missing");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies MaxRejectsTextColumn.
    /// </summary>
    private static void MaxRejectsTextColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Max("Key");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies SumRejectsOverflow.
    /// </summary>
    private static void SumRejectsOverflow()
    {
        // Setup
        var table = TestTable.Create();
        table.Rows[0]["Value"] = decimal.MaxValue;

        // Act
        Action operation = () => table
            .Sum("Value");

        // Assert
        Throws<OverflowException>(operation);
    }

    /// <summary>
    /// Verifies TypedSummaryRejectsOverflow.
    /// </summary>
    private static void TypedSummaryRejectsOverflow()
    {
        // Setup
        var table = TestTable.Create();
        table.Rows[0]["Count"] = int.MaxValue;

        // Act
        Action operation = () => table
            .Sum<short>("Count");

        // Assert
        Throws<OverflowException>(operation);
    }

    /// <summary>
    /// Verifies MeanRejectsNonfiniteValues.
    /// </summary>
    private static void MeanRejectsNonfiniteValues()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(double));
        table.Rows.Add(double.PositiveInfinity);

        // Act
        Action operation = () => table
            .Mean("Value");

        // Assert
        Throws<ArgumentException>(operation);
    }

    #endregion
}
