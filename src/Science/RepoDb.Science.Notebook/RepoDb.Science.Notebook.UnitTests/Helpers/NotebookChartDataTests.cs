namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the bucketing and building of chart data.
/// </summary>
internal static class NotebookChartDataTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        RunBucketTests();
        RunBuilderTests();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs bucket scenarios.
    /// </summary>
    private static void RunBucketTests()
    {
        CountCoversTheSpan();
        AggregateAddsTheValues();
        AggregateAveragesTheValues();
        AggregateSkipsMissingValues();
        LabelsShowHoursAndMinutes();
        LabelsShowSeconds();
        LabelsShowDays();
        WidthIsTheRequestedOne();
        WidthGrowsToTheNextBucket();
        WidthGrowsBeyondTheBuckets();
    }

    /// <summary>
    /// Runs builder scenarios.
    /// </summary>
    private static void RunBuilderTests()
    {
        BuildsTimedLabels();
        BuildsStackedSums();
        BuildsLineAverages();
        SkipsRowsWithoutATime();
        BuildsCategoryLabels();
        BuildsCategoryValues();
        LabelsAMissingCategory();
        LimitsTheCategories();
        GivesSeriesTheirColors();
    }

    /// <summary>
    /// Creates points some seconds after the epoch, each with one value.
    /// </summary>
    /// <param name="points">The seconds and values.</param>
    /// <returns>The points.</returns>
    private static NotebookChartPoint[] Points(params (int Seconds, double? Value)[] points) => points
        .Select(point => new NotebookChartPoint(DateTime.UnixEpoch.AddSeconds(point.Seconds),
            [point.Value]))
        .ToArray();

    /// <summary>
    /// Builds the data of a table.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="type">The chart type.</param>
    /// <returns>The data.</returns>
    private static NotebookChartData Build(NotebookTable table,
        NotebookChartType type = NotebookChartType.StackedLines) => NotebookChartBuilder.Build(table,
        NotebookChartValidator.Check(table,
            false),
        NotebookChartBucket.OneMinute,
        type,
        NotebookChartColors.Normalize(null));

    /// <summary>
    /// Tests CountCoversTheSpan.
    /// </summary>
    private static void CountCoversTheSpan()
    {
        // Act
        var count = NotebookChartBuckets.CountOf(Points((0, 1),
            (125, 1)),
            60);

        // Assert
        Check(count == 3);
    }

    /// <summary>
    /// Tests AggregateAddsTheValues.
    /// </summary>
    private static void AggregateAddsTheValues()
    {
        // Act
        var values = NotebookChartBuckets.Aggregate(Points((0, 2),
            (30, 4),
            (120, 6)),
            1,
            60,
            false);

        // Assert
        Check(values[0]
            .SequenceEqual([6d, 0d, 6d]));
    }

    /// <summary>
    /// Tests AggregateAveragesTheValues.
    /// </summary>
    private static void AggregateAveragesTheValues()
    {
        // Act
        var values = NotebookChartBuckets.Aggregate(Points((0, 2),
            (30, 4),
            (120, 6)),
            1,
            60,
            true);

        // Assert
        Check(values[0]
            .SequenceEqual([3d, 0d, 6d]));
    }

    /// <summary>
    /// Tests AggregateSkipsMissingValues.
    /// </summary>
    private static void AggregateSkipsMissingValues()
    {
        // Act
        var values = NotebookChartBuckets.Aggregate(Points((0, null),
            (1, 4)),
            1,
            60,
            true);

        // Assert
        Check(values[0]
            .Single() == 4);
    }

    /// <summary>
    /// Tests LabelsShowHoursAndMinutes.
    /// </summary>
    private static void LabelsShowHoursAndMinutes()
    {
        // Act
        var labels = NotebookChartBuckets.LabelsOf(Points((0, 1),
            (60, 1)),
            60);

        // Assert
        Check(labels
            .SequenceEqual(["00:00", "00:01"]));
    }

    /// <summary>
    /// Tests LabelsShowSeconds.
    /// </summary>
    private static void LabelsShowSeconds()
    {
        // Act
        var labels = NotebookChartBuckets.LabelsOf(Points((0, 1),
            (1, 1)),
            1);

        // Assert
        Check(labels[1] == "00:00:01");
    }

    /// <summary>
    /// Tests LabelsShowDays.
    /// </summary>
    private static void LabelsShowDays()
    {
        // Act
        var labels = NotebookChartBuckets.LabelsOf(Points((0, 1),
            (86400, 1)),
            86400);

        // Assert
        Check(labels[1] == "01-02 00:00");
    }

    /// <summary>
    /// Tests WidthIsTheRequestedOne.
    /// </summary>
    private static void WidthIsTheRequestedOne()
    {
        // Act
        var width = NotebookChartBuckets.WidthOf(NotebookChartBucket.FiveMinutes,
            TimeSpan.Zero);

        // Assert
        Check(width == 300);
    }

    /// <summary>
    /// Tests WidthGrowsToTheNextBucket.
    /// </summary>
    private static void WidthGrowsToTheNextBucket()
    {
        // Act
        var width = NotebookChartBuckets.WidthOf(NotebookChartBucket.OneMinute,
            TimeSpan.FromDays(10));

        // Assert
        Check(width == 300);
    }

    /// <summary>
    /// Tests WidthGrowsBeyondTheBuckets.
    /// </summary>
    private static void WidthGrowsBeyondTheBuckets()
    {
        // Setup
        var span = TimeSpan.FromDays(10000);

        // Act
        var width = NotebookChartBuckets.WidthOf(NotebookChartBucket.OneDay,
            span);

        // Assert
        Check(width % 60 == 0 && span.TotalSeconds / width <= NotebookChartBuckets.MaxBuckets);
    }

    /// <summary>
    /// Tests BuildsTimedLabels.
    /// </summary>
    private static void BuildsTimedLabels()
    {
        // Act
        var data = Build(SampleTables.Timed());

        // Assert
        Check(data.IsTimeBased && data.Labels.Count == 3);
    }

    /// <summary>
    /// Tests BuildsStackedSums.
    /// </summary>
    private static void BuildsStackedSums()
    {
        // Act
        var data = Build(SampleTables.Timed());

        // Assert
        Check(data.Series
            .Single().Points[0] == 6);
    }

    /// <summary>
    /// Tests BuildsLineAverages.
    /// </summary>
    private static void BuildsLineAverages()
    {
        // Act
        var data = Build(SampleTables.Timed(),
            NotebookChartType.Lines);

        // Assert
        Check(data.Series
            .Single().Points[0] == 3);
    }

    /// <summary>
    /// Tests SkipsRowsWithoutATime.
    /// </summary>
    private static void SkipsRowsWithoutATime()
    {
        // Setup
        var table = SampleTables.Create(["At:datetime", "Total:int"],
            [null, "100"],
            ["2026-01-01T00:00:00Z", "x"]);

        // Act
        var data = Build(table);

        // Assert
        Check(data.Series
            .Single().Points
            .Single() == 0);
    }

    /// <summary>
    /// Tests BuildsCategoryLabels.
    /// </summary>
    private static void BuildsCategoryLabels()
    {
        // Act
        var data = Build(SampleTables.Categories());

        // Assert
        Check(!data.IsTimeBased && data.Labels
            .SequenceEqual(["PH", "NL"]));
    }

    /// <summary>
    /// Tests BuildsCategoryValues.
    /// </summary>
    private static void BuildsCategoryValues()
    {
        // Setup
        var table = SampleTables.Create(["Name:string", "Total:int"],
            ["a", "7"],
            ["b", null],
            ["c"]);

        // Act
        var data = Build(table);

        // Assert
        Check(data.Series
            .Single().Points
            .SequenceEqual([7d, 0d, 0d]));
    }

    /// <summary>
    /// Tests LabelsAMissingCategory.
    /// </summary>
    private static void LabelsAMissingCategory()
    {
        // Setup
        var table = SampleTables.Create(["Name:string", "Total:int"],
            [null, "1"]);

        // Act
        var data = Build(table);

        // Assert
        Check(data.Labels
            .Single() == "(null)");
    }

    /// <summary>
    /// Tests LimitsTheCategories.
    /// </summary>
    private static void LimitsTheCategories()
    {
        // Setup
        var rows = Enumerable
            .Range(0,
                NotebookChartBuckets.MaxBuckets + 1)
            .Select(number => new string?[] { $"n{number}", "1" })
            .ToArray();

        // Act
        var data = Build(SampleTables.Create(["Name:string", "Total:int"],
            rows));

        // Assert
        Check(data.Labels.Count == NotebookChartBuckets.MaxBuckets);
    }

    /// <summary>
    /// Tests GivesSeriesTheirColors.
    /// </summary>
    private static void GivesSeriesTheirColors()
    {
        // Setup
        var table = SampleTables.Categories();
        var colors = new Dictionary<string, string>(StringComparer.Ordinal) { ["Total"] = "#000000" };

        // Act
        var data = NotebookChartBuilder.Build(table,
            NotebookChartValidator.Check(table,
                false),
            NotebookChartBucket.OneMinute,
            NotebookChartType.Bars,
            colors);

        // Assert
        Check(data.Series
            .Single() is { Name: "Total", Color: "#000000" });
    }

    #endregion
}
