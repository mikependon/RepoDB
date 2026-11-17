using System.Data;
using System.Diagnostics.CodeAnalysis;
using RepoDb.Science.UnitTests.Helpers;
using static RepoDb.Science.UnitTests.Helpers.AssertHelper;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Verifies cleaning operations and failure behavior.
/// </summary>
internal static class CleaningTests
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
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch1()
    {
        DropNullsChecksAllColumns();
        DropNullsChecksSelectedColumns();
        DropNullsSupportsInPlace();
        FillNullsCopiesByDefault();
        FillNullsSupportsInPlace();
        FillNullsAcceptsMissingReplacement();
        FillNullsRejectsInvalidValueBeforeMutation();
        DropDuplicatesUsesAllColumns();
    }

    /// <summary>
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch2()
    {
        DropDuplicatesUsesSelectedKey();
        DropDuplicatesGroupsMissingKeys();
        DropDuplicatesSupportsCopyOverload();
        ReplaceMatchesPresentValues();
        ReplaceMatchesNormalizedMissingValues();
        ReplaceSupportsInPlaceAndMissingOutput();
        CastColumnRebuildsSchema();
        CastColumnSupportsNullableTypes();
    }

    /// <summary>
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch3()
    {
        CastColumnParsesInvariantNumbers();
        CastColumnPreservesMatchingTypes();
        CastColumnRejectsBadFormat();
        CastColumnRejectsOverflow();
        CastColumnRejectsUnsupportedConversion();
        DropNullsRejectsMissingColumn();
        FillNullsRejectsMissingColumn();
        DuplicatesRejectsMissingColumn();
    }

    /// <summary>
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch4()
    {
        ReplaceRejectsMissingColumn();
        CastRejectsMissingColumn();
    }

    /// <summary>
    /// Verifies DropNullsChecksAllColumns.
    /// </summary>
    private static void DropNullsChecksAllColumns()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .DropNulls();

        // Assert
        Check(result.Rows.Count == 3 && table.Rows.Count == 4);
    }

    /// <summary>
    /// Verifies DropNullsChecksSelectedColumns.
    /// </summary>
    private static void DropNullsChecksSelectedColumns()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .DropNulls("Key");

        // Assert
        Check(result.Rows.Count == 4);
    }

    /// <summary>
    /// Verifies DropNullsSupportsInPlace.
    /// </summary>
    private static void DropNullsSupportsInPlace()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .DropNulls(new[] { "Value" },
                true);

        // Assert
        Check(ReferenceEquals(table,
            result) && table.Rows.Count == 3);
    }

    /// <summary>
    /// Verifies FillNullsCopiesByDefault.
    /// </summary>
    private static void FillNullsCopiesByDefault()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .FillNulls("Value",
                0m);

        // Assert
        Check((decimal)result.Rows[1]["Value"] == 0m && table.Rows[1].IsNull("Value"));
    }

    /// <summary>
    /// Verifies FillNullsSupportsInPlace.
    /// </summary>
    private static void FillNullsSupportsInPlace()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .FillNulls("Value",
                5m,
                true);

        // Assert
        Check(ReferenceEquals(table,
            result) && (decimal)table.Rows[1]["Value"] == 5m);
    }

    /// <summary>
    /// Verifies FillNullsAcceptsMissingReplacement.
    /// </summary>
    private static void FillNullsAcceptsMissingReplacement()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .FillNulls("Value",
                null);

        // Assert
        Check(result.Rows[1].IsNull("Value"));
    }

    /// <summary>
    /// Verifies FillNullsRejectsInvalidValueBeforeMutation.
    /// </summary>
    private static void FillNullsRejectsInvalidValueBeforeMutation()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .FillNulls("Value",
                "invalid",
                true);

        // Assert
        Throws<ArgumentException>(operation);
        Check(table.Rows[1].IsNull("Value"));
    }

    /// <summary>
    /// Verifies DropDuplicatesUsesAllColumns.
    /// </summary>
    private static void DropDuplicatesUsesAllColumns()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .DropDuplicates();

        // Assert
        Check(result.Rows.Count == 3 && table.Rows.Count == 4);
    }

    /// <summary>
    /// Verifies DropDuplicatesUsesSelectedKey.
    /// </summary>
    private static void DropDuplicatesUsesSelectedKey()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .DropDuplicates("Key");

        // Assert
        Check(result.Rows.Count == 2 && (int)result.Rows[0]["Count"] == 1);
    }

    /// <summary>
    /// Verifies DropDuplicatesGroupsMissingKeys.
    /// </summary>
    private static void DropDuplicatesGroupsMissingKeys()
    {
        // Setup
        var table = TestTable.Create();
        table.Rows[0]["Key"] = DBNull.Value;
        table.Rows[1]["Key"] = DBNull.Value;

        // Act
        var result = table
            .DropDuplicates(new[] { "Key" },
                true);

        // Assert
        Check(ReferenceEquals(table,
            result) && result.Rows.Count == 2);
    }

    /// <summary>
    /// Verifies DropDuplicatesSupportsCopyOverload.
    /// </summary>
    private static void DropDuplicatesSupportsCopyOverload()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .DropDuplicates(new[] { "Key" },
                false);

        // Assert
        Check(result.Rows.Count == 2 && table.Rows.Count == 4);
    }

    /// <summary>
    /// Verifies ReplaceMatchesPresentValues.
    /// </summary>
    private static void ReplaceMatchesPresentValues()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Replace("Key",
                "A",
                "C");

        // Assert
        Check(result.Rows[0]["Key"] is "C" && table.Rows[0]["Key"] is "A");
    }

    /// <summary>
    /// Verifies ReplaceMatchesNormalizedMissingValues.
    /// </summary>
    private static void ReplaceMatchesNormalizedMissingValues()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Replace("Value",
                null,
                4m);

        // Assert
        Check((decimal)result.Rows[1]["Value"] == 4m);
    }

    /// <summary>
    /// Verifies ReplaceSupportsInPlaceAndMissingOutput.
    /// </summary>
    private static void ReplaceSupportsInPlaceAndMissingOutput()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Replace("Value",
                30m,
                DBNull.Value,
                true);

        // Assert
        Check(ReferenceEquals(table,
            result) && result.Rows[2].IsNull("Value"));
    }

    /// <summary>
    /// Verifies CastColumnRebuildsSchema.
    /// </summary>
    private static void CastColumnRebuildsSchema()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .CastColumn<double>("Value");

        // Assert
        Check(result.Columns["Value"]!.DataType == typeof(double) && result.Columns[1].ColumnName is "Value");
        Check(result.Rows[1].IsNull("Value") && table.Columns["Value"]!.DataType == typeof(decimal));
    }

    /// <summary>
    /// Verifies CastColumnSupportsNullableTypes.
    /// </summary>
    private static void CastColumnSupportsNullableTypes()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .CastColumn<int?>("Value");

        // Assert
        Check(result.Columns["Value"]!.DataType == typeof(int) && result.Rows[1].IsNull("Value"));
    }

    /// <summary>
    /// Verifies CastColumnParsesInvariantNumbers.
    /// </summary>
    private static void CastColumnParsesInvariantNumbers()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(string));
        table.Rows.Add("1.25");

        // Act
        var result = table
            .CastColumn<decimal>("Value");

        // Assert
        Check((decimal)result.Rows[0][0] == 1.25m);
    }

    /// <summary>
    /// Verifies CastColumnPreservesMatchingTypes.
    /// </summary>
    private static void CastColumnPreservesMatchingTypes()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .CastColumn<decimal>("Value");

        // Assert
        Check((decimal)result.Rows[0]["Value"] == 10m);
    }

    /// <summary>
    /// Verifies CastColumnRejectsBadFormat.
    /// </summary>
    private static void CastColumnRejectsBadFormat()
    {
        // Setup
        var table = TestTable.Create();
        table.Rows[0]["Key"] = "invalid";

        // Act
        Action operation = () => table
            .CastColumn<int>("Key");

        // Assert
        Throws<FormatException>(operation);
    }

    /// <summary>
    /// Verifies CastColumnRejectsOverflow.
    /// </summary>
    private static void CastColumnRejectsOverflow()
    {
        // Setup
        var table = TestTable.Create();
        table.Rows[0]["Value"] = decimal.MaxValue;

        // Act
        Action operation = () => table
            .CastColumn<int>("Value");

        // Assert
        Throws<OverflowException>(operation);
    }

    /// <summary>
    /// Verifies CastColumnRejectsUnsupportedConversion.
    /// </summary>
    private static void CastColumnRejectsUnsupportedConversion()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .CastColumn<Guid>("Value");

        // Assert
        Throws<InvalidCastException>(operation);
    }

    /// <summary>
    /// Verifies DropNullsRejectsMissingColumn.
    /// </summary>
    private static void DropNullsRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .DropNulls("Missing");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies FillNullsRejectsMissingColumn.
    /// </summary>
    private static void FillNullsRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .FillNulls("Missing",
                0);

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies DuplicatesRejectsMissingColumn.
    /// </summary>
    private static void DuplicatesRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .DropDuplicates("Missing");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies ReplaceRejectsMissingColumn.
    /// </summary>
    private static void ReplaceRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Replace("Missing",
                1,
                2);

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies CastRejectsMissingColumn.
    /// </summary>
    private static void CastRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .CastColumn<int>("Missing");

        // Assert
        Throws<ArgumentException>(operation);
    }

    #endregion
}
