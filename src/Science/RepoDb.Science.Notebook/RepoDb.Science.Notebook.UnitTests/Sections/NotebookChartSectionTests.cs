namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the settings and drawing of the chart section.
/// </summary>
internal static class NotebookChartSectionTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        SectionStartsWithDefaults();
        SectionStartsWithoutData();
        SectionIsAChart();
        NullScriptsAreEmpty();
        SqlSourceRunsTheSqlScript();
        CSharpSourceRunsTheCSharpScript();
        SourceIsContent();
        UnknownSourceIsRejected();
        UnknownTypeIsRejected();
        UnknownBucketIsRejected();
        ResultsAreDrawn();
        UnusableResultsAreNotDrawn();
        ClearingRemovesTheData();
        TypeRedraws();
        BucketRedraws();
        SameTypeDoesNotRedraw();
        SameBucketDoesNotRedraw();
        ColorsAreNormalized();
        ColorsRedraw();
        ColorsAreContent();
        SameColorsAreNotAChange();
        NullColorsAreRejected();
        FailingOnNonNumericRedraws();
        SameFailSettingDoesNotRedraw();
        TypeAndBucketAreContent();
        ConnectionIsContent();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates a chart that draws a table.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <returns>The chart.</returns>
    private static NotebookChartSection Drawn(NotebookTable? table = null)
    {
        var section = new NotebookChartSection();
        section.RestoreResults("",
            [table ?? SampleTables.Timed()],
            "");
        return section;
    }

    /// <summary>
    /// Creates colors for the total series.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>The colors.</returns>
    private static Dictionary<string, string> Total(string color) => new(StringComparer.Ordinal) { ["Total"] = color };

    /// <summary>
    /// Tests SectionStartsWithDefaults.
    /// </summary>
    private static void SectionStartsWithDefaults()
    {
        // Act
        var section = new NotebookChartSection();

        // Assert
        Check(section.Source == NotebookCodeLanguage.Sql && section.ChartType == NotebookChartType.StackedLines);
        Check(section.Bucket == NotebookChartBucket.OneMinute && section.Colors.Count == 0 && !section.FailOnNonNumeric);
    }

    /// <summary>
    /// Tests SectionStartsWithoutData.
    /// </summary>
    private static void SectionStartsWithoutData()
    {
        // Act
        var section = new NotebookChartSection();

        // Assert
        Check(section.Table is null && section.Data is null);
    }

    /// <summary>
    /// Tests SectionIsAChart.
    /// </summary>
    private static void SectionIsAChart()
    {
        // Act
        var section = new NotebookChartSection();

        // Assert
        Check(section.Type == NotebookSectionType.Chart);
    }

    /// <summary>
    /// Tests NullScriptsAreEmpty.
    /// </summary>
    private static void NullScriptsAreEmpty()
    {
        // Act
        var section = new NotebookChartSection { SqlScript = null!, CSharpScript = null! };

        // Assert
        Check(section.SqlScript.Length == 0 && section.CSharpScript.Length == 0);
    }

    /// <summary>
    /// Tests SqlSourceRunsTheSqlScript.
    /// </summary>
    private static void SqlSourceRunsTheSqlScript()
    {
        // Act
        var section = new NotebookChartSection { SqlScript = "sql", CSharpScript = "code" };

        // Assert
        Check(section.ScriptToRun == "sql" && section.ScriptLanguage == NotebookCodeLanguage.Sql);
    }

    /// <summary>
    /// Tests CSharpSourceRunsTheCSharpScript.
    /// </summary>
    private static void CSharpSourceRunsTheCSharpScript()
    {
        // Act
        var section = new NotebookChartSection { SqlScript = "sql", CSharpScript = "code", Source = NotebookCodeLanguage.CSharp };

        // Assert
        Check(section.ScriptToRun == "code" && section.ScriptLanguage == NotebookCodeLanguage.CSharp);
    }

    /// <summary>
    /// Tests SourceIsContent.
    /// </summary>
    private static void SourceIsContent()
    {
        // Setup
        var section = new NotebookChartSection();

        // Act
        section.Source = NotebookCodeLanguage.CSharp;

        // Assert
        Check(section.HasChanges);
    }

    /// <summary>
    /// Tests UnknownSourceIsRejected.
    /// </summary>
    private static void UnknownSourceIsRejected()
    {
        // Act
        Action operation = () => _ = new NotebookChartSection { Source = (NotebookCodeLanguage)9 };

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Tests UnknownTypeIsRejected.
    /// </summary>
    private static void UnknownTypeIsRejected()
    {
        // Act
        Action operation = () => _ = new NotebookChartSection { ChartType = (NotebookChartType)9 };

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Tests UnknownBucketIsRejected.
    /// </summary>
    private static void UnknownBucketIsRejected()
    {
        // Act
        Action operation = () => _ = new NotebookChartSection { Bucket = (NotebookChartBucket)9 };

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Tests ResultsAreDrawn.
    /// </summary>
    private static void ResultsAreDrawn()
    {
        // Act
        var section = Drawn();

        // Assert
        Check(section.Table is not null && section.Data is { Series.Count: 1 });
    }

    /// <summary>
    /// Tests UnusableResultsAreNotDrawn.
    /// </summary>
    private static void UnusableResultsAreNotDrawn()
    {
        // Act
        var section = Drawn(SampleTables.Create(["a:int"],
            ["1"]));

        // Assert
        Check(section.Table is not null && section.Data is null);
    }

    /// <summary>
    /// Tests ClearingRemovesTheData.
    /// </summary>
    private static void ClearingRemovesTheData()
    {
        // Setup
        var section = Drawn();

        // Act
        section.ClearOutput();

        // Assert
        Check(section.Table is null && section.Data is null);
    }

    /// <summary>
    /// Tests TypeRedraws.
    /// </summary>
    private static void TypeRedraws()
    {
        // Setup
        var section = Drawn();

        // Act
        section.ChartType = NotebookChartType.Lines;

        // Assert
        Check(section.Data!.Series[0].Points[0] == 3);
    }

    /// <summary>
    /// Tests BucketRedraws.
    /// </summary>
    private static void BucketRedraws()
    {
        // Setup
        var section = Drawn();

        // Act
        section.Bucket = NotebookChartBucket.FiveMinutes;

        // Assert
        Check(section.Data!.Labels.Count == 1);
    }

    /// <summary>
    /// Tests SameTypeDoesNotRedraw.
    /// </summary>
    private static void SameTypeDoesNotRedraw()
    {
        // Setup
        var section = Drawn();
        var data = section.Data;

        // Act
        section.ChartType = NotebookChartType.StackedLines;

        // Assert
        Check(ReferenceEquals(section.Data,
            data));
    }

    /// <summary>
    /// Tests SameBucketDoesNotRedraw.
    /// </summary>
    private static void SameBucketDoesNotRedraw()
    {
        // Setup
        var section = Drawn();
        var data = section.Data;

        // Act
        section.Bucket = NotebookChartBucket.OneMinute;

        // Assert
        Check(ReferenceEquals(section.Data,
            data));
    }

    /// <summary>
    /// Tests ColorsAreNormalized.
    /// </summary>
    private static void ColorsAreNormalized()
    {
        // Setup
        var section = new NotebookChartSection();

        // Act
        section.SetColors(Total("#abcdef"));

        // Assert
        Check(section.Colors["Total"] == "#ABCDEF");
    }

    /// <summary>
    /// Tests ColorsRedraw.
    /// </summary>
    private static void ColorsRedraw()
    {
        // Setup
        var section = Drawn();

        // Act
        section.SetColors(Total("#000000"));

        // Assert
        Check(section.Data!.Series[0].Color == "#000000");
    }

    /// <summary>
    /// Tests ColorsAreContent.
    /// </summary>
    private static void ColorsAreContent()
    {
        // Setup
        var section = new NotebookChartSection();
        var recorder = new PropertyRecorder(section);

        // Act
        section.SetColors(Total("#000000"));

        // Assert
        Check(section.HasChanges && recorder.Names.Contains(nameof(NotebookChartSection.Colors)));
    }

    /// <summary>
    /// Tests SameColorsAreNotAChange.
    /// </summary>
    private static void SameColorsAreNotAChange()
    {
        // Setup
        var section = new NotebookChartSection();
        var recorder = new PropertyRecorder(section);

        // Act
        section.SetColors(Total("bad"));

        // Assert
        Check(!section.HasChanges && recorder.Names.Count == 0);
    }

    /// <summary>
    /// Tests NullColorsAreRejected.
    /// </summary>
    private static void NullColorsAreRejected()
    {
        // Setup
        var section = new NotebookChartSection();

        // Act
        Action operation = () => section.SetColors(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests FailingOnNonNumericRedraws.
    /// </summary>
    private static void FailingOnNonNumericRedraws()
    {
        // Setup
        var section = Drawn(SampleTables.Mixed());

        // Act
        section.FailOnNonNumeric = true;

        // Assert
        Check(section.Data is null && !section.HasChanges);
    }

    /// <summary>
    /// Tests SameFailSettingDoesNotRedraw.
    /// </summary>
    private static void SameFailSettingDoesNotRedraw()
    {
        // Setup
        var section = Drawn();
        var data = section.Data;

        // Act
        section.FailOnNonNumeric = false;

        // Assert
        Check(ReferenceEquals(section.Data,
            data));
    }

    /// <summary>
    /// Tests ConnectionIsContent.
    /// </summary>
    private static void ConnectionIsContent()
    {
        // Setup
        var section = new NotebookChartSection { ConnectionId = Guid.NewGuid() };
        section.MarkSaved();

        // Act
        section.ConnectionId = Guid.NewGuid();

        // Assert
        Check(section.HasChanges);
    }

    /// <summary>
    /// Tests TypeAndBucketAreContent.
    /// </summary>
    private static void TypeAndBucketAreContent()
    {
        // Setup
        var first = new NotebookChartSection();
        var second = new NotebookChartSection();

        // Act
        first.ChartType = NotebookChartType.Bars;
        second.Bucket = NotebookChartBucket.OneDay;

        // Assert
        Check(first.HasChanges && second.HasChanges);
    }

    #endregion
}
