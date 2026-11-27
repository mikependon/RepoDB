namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the names, colors, values and validation of charts.
/// </summary>
internal static class NotebookChartHelperTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        RunNameTests();
        RunColorTests();
        RunValueTests();
        RunValidatorTests();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs name scenarios.
    /// </summary>
    private static void RunNameTests()
    {
        ChartTypesHaveTheirNames();
        BucketsHaveTheirNames();
        NamesParseBack();
        UnknownNamesParseToDefaults();
        UnknownTypeHasNoName();
        UnknownBucketHasNoName();
    }

    /// <summary>
    /// Runs color scenarios.
    /// </summary>
    private static void RunColorTests()
    {
        ColorIsTrimmedAndUppercased();
        InvalidColorsAreRejected();
        NormalizeKeepsValidColors();
        NormalizeOfNothingIsEmpty();
        NormalizedColorsAreReadOnly();
        SnapshotIgnoresOrder();
    }

    /// <summary>
    /// Runs value scenarios.
    /// </summary>
    private static void RunValueTests()
    {
        NumericTypesAreRecognized();
        TimeTypesAreRecognized();
        TextAtReadsWithinTheRow();
        FiniteNumbersAreRead();
        OtherNumbersAreRejected();
        TimesAreReadAsUniversal();
        OtherTimesAreRejected();
    }

    /// <summary>
    /// Runs validation scenarios.
    /// </summary>
    private static void RunValidatorTests()
    {
        RejectsATableWithoutRows();
        RejectsATableWithOneColumn();
        RejectsTimesThatCannotBeRead();
        TextTimesAreTimeBased();
        OtherTextsAreCategories();
        MissingTextsAreCategories();
        ShortRowsAreAccepted();
        SkipsColumnsThatAreNotNumeric();
        FailsOnNonNumericWhenAsked();
        PassesWhenAllColumnsAreNumeric();
        RejectsATableWithoutNumbers();
    }

    /// <summary>
    /// Checks a table.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <returns>The outcome.</returns>
    private static NotebookChartCheck CheckOf(NotebookTable table) => NotebookChartValidator.Check(table,
            false,
            NotebookMessages.Default);

    /// <summary>
    /// Tests ChartTypesHaveTheirNames.
    /// </summary>
    private static void ChartTypesHaveTheirNames()
    {
        // Act
        var names = Enum.GetValues<NotebookChartType>()
            .Select(NotebookChartNames.NameOf);

        // Assert
        Check(string.Join('|',
            names) == "Lines (Stacked)|Lines|Bars|Bars (Stacked)");
    }

    /// <summary>
    /// Tests BucketsHaveTheirNames.
    /// </summary>
    private static void BucketsHaveTheirNames()
    {
        // Act
        var names = Enum.GetValues<NotebookChartBucket>()
            .Select(NotebookChartNames.NameOf);

        // Assert
        Check(string.Join('|',
            names) == "1min|5min|10min|1h|12h|1d");
    }

    /// <summary>
    /// Tests NamesParseBack.
    /// </summary>
    private static void NamesParseBack()
    {
        // Assert
        Check(NotebookChartNames.ParseType("Bars") == NotebookChartType.Bars);
        Check(NotebookChartNames.ParseBucket("1h") == NotebookChartBucket.OneHour);
    }

    /// <summary>
    /// Tests UnknownNamesParseToDefaults.
    /// </summary>
    private static void UnknownNamesParseToDefaults()
    {
        // Assert
        Check(NotebookChartNames.ParseType("Pie") == NotebookChartType.StackedLines);
        Check(NotebookChartNames.ParseBucket(null) == NotebookChartBucket.OneMinute);
    }

    /// <summary>
    /// Tests UnknownTypeHasNoName.
    /// </summary>
    private static void UnknownTypeHasNoName()
    {
        // Act
        Action operation = () => NotebookChartNames.NameOf((NotebookChartType)9);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Tests UnknownBucketHasNoName.
    /// </summary>
    private static void UnknownBucketHasNoName()
    {
        // Act
        Action operation = () => NotebookChartNames.NameOf((NotebookChartBucket)9);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Tests ColorIsTrimmedAndUppercased.
    /// </summary>
    private static void ColorIsTrimmedAndUppercased()
    {
        // Act
        var color = NotebookChartColors.NormalizeColor(" #a1b2c3 ");

        // Assert
        Check(color == "#A1B2C3");
    }

    /// <summary>
    /// Tests InvalidColorsAreRejected.
    /// </summary>
    private static void InvalidColorsAreRejected()
    {
        // Setup
        string?[] colors = [null, "#12345", "1234567", "#12345G"];

        // Assert
        Check(colors
            .All(color => NotebookChartColors.NormalizeColor(color) is null));
    }

    /// <summary>
    /// Tests NormalizeKeepsValidColors.
    /// </summary>
    private static void NormalizeKeepsValidColors()
    {
        // Setup
        var colors = new Dictionary<string, string>(StringComparer.Ordinal) { ["a"] = "#abcdef", ["b"] = "red" };

        // Act
        var result = NotebookChartColors.Normalize(colors);

        // Assert
        Check(result.Count == 1 && result["a"] == "#ABCDEF");
    }

    /// <summary>
    /// Tests NormalizeOfNothingIsEmpty.
    /// </summary>
    private static void NormalizeOfNothingIsEmpty()
    {
        // Act
        var result = NotebookChartColors.Normalize(null);

        // Assert
        Check(result.Count == 0);
    }

    /// <summary>
    /// Tests NormalizedColorsAreReadOnly.
    /// </summary>
    private static void NormalizedColorsAreReadOnly()
    {
        // Setup
        var colors = (IDictionary<string, string>)NotebookChartColors.Normalize(null);

        // Act
        Action operation = () => colors["a"] = "#000000";

        // Assert
        Throws<NotSupportedException>(operation);
    }

    /// <summary>
    /// Tests SnapshotIgnoresOrder.
    /// </summary>
    private static void SnapshotIgnoresOrder()
    {
        // Setup
        var first = new Dictionary<string, string>(StringComparer.Ordinal) { ["a"] = "1", ["b"] = "2" };
        var second = new Dictionary<string, string>(StringComparer.Ordinal) { ["b"] = "2", ["a"] = "1" };

        // Assert
        Check(NotebookChartColors.Snapshot(first) == NotebookChartColors.Snapshot(second));
    }

    /// <summary>
    /// Tests NumericTypesAreRecognized.
    /// </summary>
    private static void NumericTypesAreRecognized()
    {
        // Setup
        string[] types = ["int", "long", "short", "byte", "decimal", "double", "float"];

        // Assert
        Check(types
            .All(NotebookChartValues.IsNumeric) && !NotebookChartValues.IsNumeric("string"));
    }

    /// <summary>
    /// Tests TimeTypesAreRecognized.
    /// </summary>
    private static void TimeTypesAreRecognized()
    {
        // Assert
        Check(NotebookChartValues.IsTimestamp("datetime") && NotebookChartValues.IsTimestamp("datetimeoffset"));
        Check(!NotebookChartValues.IsTimestamp("timespan"));
    }

    /// <summary>
    /// Tests TextAtReadsWithinTheRow.
    /// </summary>
    private static void TextAtReadsWithinTheRow()
    {
        // Assert
        Check(NotebookChartValues.TextAt(["a"],
            0) == "a" && NotebookChartValues.TextAt(["a"],
                1) is null);
    }

    /// <summary>
    /// Tests FiniteNumbersAreRead.
    /// </summary>
    private static void FiniteNumbersAreRead()
    {
        // Act
        var read = NotebookChartValues.TryNumber("1.5",
            out var value);

        // Assert
        Check(read && value == 1.5);
    }

    /// <summary>
    /// Tests OtherNumbersAreRejected.
    /// </summary>
    private static void OtherNumbersAreRejected()
    {
        // Setup
        string?[] texts = [null, "x", "Infinity", "NaN"];

        // Assert
        Check(texts
            .All(text => !NotebookChartValues.TryNumber(text,
                out _)));
    }

    /// <summary>
    /// Tests TimesAreReadAsUniversal.
    /// </summary>
    private static void TimesAreReadAsUniversal()
    {
        // Act
        var read = NotebookChartValues.TryTimestamp("1970-01-01T02:00:00+02:00",
            out var value);

        // Assert
        Check(read && value == DateTime.UnixEpoch);
    }

    /// <summary>
    /// Tests OtherTimesAreRejected.
    /// </summary>
    private static void OtherTimesAreRejected()
    {
        // Assert
        Check(!NotebookChartValues.TryTimestamp(null,
            out _) && !NotebookChartValues.TryTimestamp("soon",
                out _));
    }

    /// <summary>
    /// Tests RejectsATableWithoutRows.
    /// </summary>
    private static void RejectsATableWithoutRows()
    {
        // Act
        var check = CheckOf(SampleTables.Create(["a:string", "b:int"]));

        // Assert
        Check(check.Error == "The result has no rows to chart.");
    }

    /// <summary>
    /// Tests RejectsATableWithOneColumn.
    /// </summary>
    private static void RejectsATableWithOneColumn()
    {
        // Act
        var check = CheckOf(SampleTables.Create(["a:int"],
            ["1"]));

        // Assert
        Check(!check.IsValid);
    }

    /// <summary>
    /// Tests RejectsTimesThatCannotBeRead.
    /// </summary>
    private static void RejectsTimesThatCannotBeRead()
    {
        // Act
        var check = CheckOf(SampleTables.Create(["a:datetime", "b:int"],
            [null, "1"]));

        // Assert
        Check(check.Error == "No row has a valid timestamp in its first column.");
    }

    /// <summary>
    /// Tests TextTimesAreTimeBased.
    /// </summary>
    private static void TextTimesAreTimeBased()
    {
        // Act
        var check = CheckOf(SampleTables.Create(["a:string", "b:int"],
            ["2026-01-01", "1"],
            [null, "2"]));

        // Assert
        Check(check.IsValid && check.IsTimeBased);
    }

    /// <summary>
    /// Tests OtherTextsAreCategories.
    /// </summary>
    private static void OtherTextsAreCategories()
    {
        // Act
        var check = CheckOf(SampleTables.Categories());

        // Assert
        Check(check.IsValid && !check.IsTimeBased);
    }

    /// <summary>
    /// Tests MissingTextsAreCategories.
    /// </summary>
    private static void MissingTextsAreCategories()
    {
        // Act
        var check = CheckOf(SampleTables.Create(["a:string", "b:int"],
            [null, "1"]));

        // Assert
        Check(check.IsValid && !check.IsTimeBased);
    }

    /// <summary>
    /// Tests ShortRowsAreAccepted.
    /// </summary>
    private static void ShortRowsAreAccepted()
    {
        // Act
        var check = CheckOf(SampleTables.Create(["a:int", "b:int"],
            Array.Empty<string?>()));

        // Assert
        Check(check.IsValid && !check.IsTimeBased);
    }

    /// <summary>
    /// Tests SkipsColumnsThatAreNotNumeric.
    /// </summary>
    private static void SkipsColumnsThatAreNotNumeric()
    {
        // Act
        var check = CheckOf(SampleTables.Mixed());

        // Assert
        Check(check.ValueColumns
            .Single() == 1 && check.Skipped
            .Single() == "Note");
    }

    /// <summary>
    /// Tests FailsOnNonNumericWhenAsked.
    /// </summary>
    private static void FailsOnNonNumericWhenAsked()
    {
        // Act
        var check = NotebookChartValidator.Check(SampleTables.Mixed(),
            true,
            NotebookMessages.Default);

        // Assert
        Check(check.Error?.StartsWith("The column 'Note' is not numeric",
            StringComparison.Ordinal) == true);
    }

    /// <summary>
    /// Tests PassesWhenAllColumnsAreNumeric.
    /// </summary>
    private static void PassesWhenAllColumnsAreNumeric()
    {
        // Act
        var check = NotebookChartValidator.Check(SampleTables.Timed(),
            true,
            NotebookMessages.Default);

        // Assert
        Check(check.IsValid);
    }

    /// <summary>
    /// Tests RejectsATableWithoutNumbers.
    /// </summary>
    private static void RejectsATableWithoutNumbers()
    {
        // Act
        var check = CheckOf(SampleTables.Create(["a:string", "b:string"],
            ["x", "y"]));

        // Assert
        Check(check.Error == "The result has no numeric column after the first one.");
    }

    #endregion
}
