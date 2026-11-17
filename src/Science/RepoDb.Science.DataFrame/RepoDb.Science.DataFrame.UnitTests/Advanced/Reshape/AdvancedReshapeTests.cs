using System.Data;
using RepoDb.Science.UnitTests.Helpers;
using static RepoDb.Science.UnitTests.Helpers.AssertHelper;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Verifies reshape additions and failure behavior.
/// </summary>
internal static class AdvancedReshapeTests
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
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch1()
    {
        PivotBuildsSparseTypedColumns();
        PivotRejectsDuplicatePairs();
        PivotTableAggregatesDuplicatePairs();
        PivotTableDefaultsToMean();
        PivotTableCanCountNonnumericValues();
        PivotSupportsMissingRowAndColumnKeys();
        PivotHandlesEmptySource();
        PivotSkipsDeletedDuplicate();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch2()
    {
        PivotRejectsAmbiguousStringifiedKeys();
        PivotRejectsBlankDynamicName();
        PivotRejectsDynamicNameCollision();
        PivotRejectsMissingKey();
        PivotTableRejectsInvalidFunction();
        PivotTableRejectsTextMean();
        ExplodeExpandsCollectionsAndMissingElements();
        ExplodePreservesScalarsStringsAndEmptyCollections();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch3()
    {
        ExplodeHandlesEmptySource();
        MeltDefaultsToNonIdentifierColumns();
        MeltSupportsExplicitValuesAndLabels();
        MeltSupportsNoIdentifiers();
        MeltSupportsNoValues();
        MeltRejectsOverlappingSelections();
        MeltRejectsNullIdentifiers();
        MeltRejectsOutputCollision();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch4()
    {
        MeltRejectsDuplicateIdentifiers();
        TransposePreservesMixedCellsAsObjects();
        TransposeHandlesZeroSourceRows();
        TransposeHandlesZeroSourceColumns();
        TransposeRejectsBlankPrefix();
        TransposeRejectsBlankRowLabel();
        TransposeRejectsLabelCollision();
    }

    /// <summary>
    /// Verifies pivot builds sparse typed columns.
    /// </summary>
    private static void PivotBuildsSparseTypedColumns()
    {
        // Setup
        var table = TestTable.Create();
        table.Rows.RemoveAt(3);

        // Act
        var result = table
            .Pivot("Key",
                "Count",
                "Value");

        // Assert
        Check(result.Rows.Count == 2 && result.Columns.Count == 4 && result.Columns["1"]!.DataType == typeof(decimal));
        Check((decimal)result.Rows[0]["1"] == 10m && result.Rows[1]
            .IsNull("1"));
    }

    /// <summary>
    /// Verifies pivot rejects duplicate pairs.
    /// </summary>
    private static void PivotRejectsDuplicatePairs()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Pivot("Key",
                "Count",
                "Value");

        // Assert
        Throws<InvalidOperationException>(operation);
        Check(table.Rows.Count == 4);
    }

    /// <summary>
    /// Verifies pivot table aggregates duplicate pairs.
    /// </summary>
    private static void PivotTableAggregatesDuplicatePairs()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .PivotTable("Key",
                "Count",
                "Value",
                AggregateFunction.Sum);

        // Assert
        Check((decimal)result.Rows[1]["3"] == 60m && result.Rows[0]
            .IsNull("2"));
    }

    /// <summary>
    /// Verifies pivot table defaults to mean.
    /// </summary>
    private static void PivotTableDefaultsToMean()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .PivotTable("Key",
                "Count",
                "Value");

        // Assert
        Check(result.Columns["3"]!.DataType == typeof(double) && (double)result.Rows[1]["3"] == 30);
    }

    /// <summary>
    /// Verifies pivot table can count nonnumeric values.
    /// </summary>
    private static void PivotTableCanCountNonnumericValues()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .PivotTable("Key",
                "Count",
                "Key",
                AggregateFunction.Count);

        // Assert
        Check(result.Columns["3"]!.DataType == typeof(int) && (int)result.Rows[1]["3"] == 2);
    }

    /// <summary>
    /// Verifies pivot supports missing row and column keys.
    /// </summary>
    private static void PivotSupportsMissingRowAndColumnKeys()
    {
        // Setup
        var table = TestTable.Create();
        table.Rows.RemoveAt(3);
        table.Rows[0]["Key"] = DBNull.Value;
        table.Rows[0]["Count"] = DBNull.Value;

        // Act
        var result = table
            .Pivot("Key",
                "Count",
                "Value");

        // Assert
        Check(result.Rows[0]
            .IsNull("Key") && (decimal)result.Rows[0]["(null)"] == 10m);
    }

    /// <summary>
    /// Verifies pivot handles empty source.
    /// </summary>
    private static void PivotHandlesEmptySource()
    {
        // Setup
        var table = TestTable.Create()
            .Clone();

        // Act
        var result = table
            .Pivot("Key",
                "Count",
                "Value");

        // Assert
        Check(result.Rows.Count == 0 && result.Columns.Count == 1);
    }

    /// <summary>
    /// Verifies pivot skips deleted duplicate.
    /// </summary>
    private static void PivotSkipsDeletedDuplicate()
    {
        // Setup
        var table = TestTable.Create();
        table.AcceptChanges();
        table.Rows[3].Delete();

        // Act
        var result = table
            .Pivot("Key",
                "Count",
                "Value");

        // Assert
        Check(result.Rows.Count == 2 && (decimal)result.Rows[1]["3"] == 30m);
    }

    /// <summary>
    /// Verifies pivot rejects ambiguous stringified keys.
    /// </summary>
    private static void PivotRejectsAmbiguousStringifiedKeys()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Index",
            typeof(int));
        table.Columns.Add("Key",
            typeof(object));
        table.Columns.Add("Value",
            typeof(int));
        table.Rows.Add(1,
            1,
            2);
        table.Rows.Add(2,
            "1",
            3);

        // Act
        Action operation = () => table
            .Pivot("Index",
                "Key",
                "Value");

        // Assert
        Throws<DuplicateNameException>(operation);
    }

    /// <summary>
    /// Verifies pivot rejects blank dynamic name.
    /// </summary>
    private static void PivotRejectsBlankDynamicName()
    {
        // Setup
        var table = TestTable.Create();
        table.Rows[0]["Key"] = " ";

        // Act
        Action operation = () => table
            .Pivot("Count",
                "Key",
                "Value");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies pivot rejects dynamic name collision.
    /// </summary>
    private static void PivotRejectsDynamicNameCollision()
    {
        // Setup
        var table = TestTable.Create();
        table.Rows[0]["Key"] = "Count";

        // Act
        Action operation = () => table
            .Pivot("Count",
                "Key",
                "Value");

        // Assert
        Throws<DuplicateNameException>(operation);
    }

    /// <summary>
    /// Verifies pivot rejects missing key.
    /// </summary>
    private static void PivotRejectsMissingKey()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Pivot("Missing",
                "Count",
                "Value");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies pivot table rejects invalid function.
    /// </summary>
    private static void PivotTableRejectsInvalidFunction()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .PivotTable("Key",
                "Count",
                "Value",
                (AggregateFunction)999);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies pivot table rejects text mean.
    /// </summary>
    private static void PivotTableRejectsTextMean()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .PivotTable("Count",
                "Key",
                "Key");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies explode expands collections and missing elements.
    /// </summary>
    private static void ExplodeExpandsCollectionsAndMissingElements()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Items",
            typeof(object));
        table.Columns.Add("Id",
            typeof(int));
        table.Rows.Add((object)new object?[] { 1, null, 2 },
            7);

        // Act
        var result = table
            .Explode("Items");

        // Assert
        Check(result.Rows.Count == 3 && result.Rows[1]
            .IsNull("Items"));
        Check((int)result.Rows[2]["Id"] == 7 && table.Rows.Count == 1);
    }

    /// <summary>
    /// Verifies explode preserves scalars strings and empty collections.
    /// </summary>
    private static void ExplodePreservesScalarsStringsAndEmptyCollections()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Items",
            typeof(object));
        table.Rows.Add((object)Array.Empty<int>());
        table.Rows.Add("AB");
        table.Rows.Add(5);
        table.Rows.Add(DBNull.Value);

        // Act
        var result = table
            .Explode("Items");

        // Assert
        Check(result.Rows.Count == 4 && result.Rows[0]
            .IsNull(0) && result.Rows[1][0] is "AB");
        Check(result.Rows[2][0] is 5 && result.Rows[3]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies explode handles empty source.
    /// </summary>
    private static void ExplodeHandlesEmptySource()
    {
        // Setup
        var table = TestTable.Create()
            .Clone();

        // Act
        var result = table
            .Explode("Key");

        // Assert
        Check(result.Rows.Count == 0 && result.Columns["Key"]!.DataType == typeof(object));
    }

    /// <summary>
    /// Verifies melt defaults to non identifier columns.
    /// </summary>
    private static void MeltDefaultsToNonIdentifierColumns()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Melt(new[] { "Key" });

        // Assert
        Check(result.Rows.Count == 8 && result.Rows[0]["Variable"] is "Value" && result.Rows[4]["Variable"] is "Count");
        Check(result.Rows[0]["Value"] is decimal value && value == 10m && result.Rows[1]
            .IsNull("Value"));
    }

    /// <summary>
    /// Verifies melt supports explicit values and labels.
    /// </summary>
    private static void MeltSupportsExplicitValuesAndLabels()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Melt(new[] { "Key" },
                new[] { "Count" },
                "Feature",
                "Measurement");

        // Assert
        Check(result.Rows.Count == 4 && result.Rows[0]["Feature"] is "Count" && result.Rows[0]["Measurement"] is 1);
    }

    /// <summary>
    /// Verifies melt supports no identifiers.
    /// </summary>
    private static void MeltSupportsNoIdentifiers()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Melt(Array.Empty<string>(),
                new[] { "Key" });

        // Assert
        Check(result.Columns.Count == 2 && result.Rows[0]["Value"] is "A");
    }

    /// <summary>
    /// Verifies melt supports no values.
    /// </summary>
    private static void MeltSupportsNoValues()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Melt(new[] { "Key" },
                Array.Empty<string>());

        // Assert
        Check(result.Rows.Count == 0 && result.Columns.Count == 3);
    }

    /// <summary>
    /// Verifies melt rejects overlapping selections.
    /// </summary>
    private static void MeltRejectsOverlappingSelections()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Melt(new[] { "Key" },
                new[] { "Key" });

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies melt rejects null identifiers.
    /// </summary>
    private static void MeltRejectsNullIdentifiers()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Melt(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies melt rejects output collision.
    /// </summary>
    private static void MeltRejectsOutputCollision()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Melt(new[] { "Key" },
                variableName: "Key");

        // Assert
        Throws<DuplicateNameException>(operation);
    }

    /// <summary>
    /// Verifies melt rejects duplicate identifiers.
    /// </summary>
    private static void MeltRejectsDuplicateIdentifiers()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Melt(new[] { "Key", "Key" });

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies transpose preserves mixed cells as objects.
    /// </summary>
    private static void TransposePreservesMixedCellsAsObjects()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Transpose();

        // Assert
        Check(result.Rows.Count == 3 && result.Columns.Count == 5 && result.Columns["Row0"]!.DataType == typeof(object));
        Check(result.Rows[0]["Column"] is "Key" && result.Rows[0]["Row0"] is "A" && result.Rows[1]
            .IsNull("Row1"));
    }

    /// <summary>
    /// Verifies transpose handles zero source rows.
    /// </summary>
    private static void TransposeHandlesZeroSourceRows()
    {
        // Setup
        var table = TestTable.Create()
            .Clone();

        // Act
        var result = table
            .Transpose("Field",
                "Record");

        // Assert
        Check(result.Rows.Count == 3 && result.Columns.Count == 1 && result.Columns[0].ColumnName is "Field");
    }

    /// <summary>
    /// Verifies transpose handles zero source columns.
    /// </summary>
    private static void TransposeHandlesZeroSourceColumns()
    {
        // Setup
        var table = TestTable.Create()
            .SelectColumns();

        // Act
        var result = table
            .Transpose();

        // Assert
        Check(result.Rows.Count == 0 && result.Columns.Count == 5);
    }

    /// <summary>
    /// Verifies transpose rejects blank prefix.
    /// </summary>
    private static void TransposeRejectsBlankPrefix()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Transpose(columnPrefix: "");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies transpose rejects blank row label.
    /// </summary>
    private static void TransposeRejectsBlankRowLabel()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Transpose(rowName: "");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies transpose rejects label collision.
    /// </summary>
    private static void TransposeRejectsLabelCollision()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Transpose(rowName: "Row0");

        // Assert
        Throws<DuplicateNameException>(operation);
    }

    #endregion
}
