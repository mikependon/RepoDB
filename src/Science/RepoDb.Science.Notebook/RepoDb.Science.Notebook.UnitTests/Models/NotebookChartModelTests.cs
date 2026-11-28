namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the chart models.
/// </summary>
internal static class NotebookChartModelTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        SeriesKeepsItsValues();
        SeriesHasNoColorByDefault();
        SeriesRejectsNullName();
        SeriesRejectsNullPoints();
        DataKeepsItsValues();
        DataRejectsNullLabels();
        DataRejectsNullSeries();
        CheckKeepsItsColumns();
        CheckIsValidWithoutError();
        FailureCarriesTheError();
        PointKeepsItsValues();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Tests SeriesKeepsItsValues.
    /// </summary>
    private static void SeriesKeepsItsValues()
    {
        // Setup
        double[] points = [1];

        // Act
        var series = new NotebookChartSeries("Total",
            points,
            "#000000");

        // Assert
        Check(series.Name == "Total" && ReferenceEquals(series.Points,
            points) && series.Color == "#000000");
    }

    /// <summary>
    /// Tests SeriesHasNoColorByDefault.
    /// </summary>
    private static void SeriesHasNoColorByDefault()
    {
        // Act
        var series = new NotebookChartSeries("Total",
            []);

        // Assert
        Check(series.Color is null);
    }

    /// <summary>
    /// Tests SeriesRejectsNullName.
    /// </summary>
    private static void SeriesRejectsNullName()
    {
        // Act
        Action operation = () => _ = new NotebookChartSeries(null!,
            []);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests SeriesRejectsNullPoints.
    /// </summary>
    private static void SeriesRejectsNullPoints()
    {
        // Act
        Action operation = () => _ = new NotebookChartSeries("n",
            null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests DataKeepsItsValues.
    /// </summary>
    private static void DataKeepsItsValues()
    {
        // Setup
        string[] labels = ["a"];
        NotebookChartSeries[] series = [];

        // Act
        var data = new NotebookChartData(labels,
            series,
            true);

        // Assert
        Check(ReferenceEquals(data.Labels,
            labels) && ReferenceEquals(data.Series,
                series) && data.IsTimeBased);
    }

    /// <summary>
    /// Tests DataRejectsNullLabels.
    /// </summary>
    private static void DataRejectsNullLabels()
    {
        // Act
        Action operation = () => _ = new NotebookChartData(null!,
            [],
            false);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests DataRejectsNullSeries.
    /// </summary>
    private static void DataRejectsNullSeries()
    {
        // Act
        Action operation = () => _ = new NotebookChartData([],
            null!,
            false);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests CheckKeepsItsColumns.
    /// </summary>
    private static void CheckKeepsItsColumns()
    {
        // Act
        var check = new NotebookChartCheck(null,
            [1],
            ["Note"],
            true);

        // Assert
        Check(check.ValueColumns
            .Single() == 1 && check.Skipped
            .Single() == "Note" && check.IsTimeBased);
    }

    /// <summary>
    /// Tests CheckIsValidWithoutError.
    /// </summary>
    private static void CheckIsValidWithoutError()
    {
        // Act
        var check = new NotebookChartCheck(null,
            [],
            [],
            false);

        // Assert
        Check(check.IsValid && check.Error is null);
    }

    /// <summary>
    /// Tests FailureCarriesTheError.
    /// </summary>
    private static void FailureCarriesTheError()
    {
        // Act
        var check = NotebookChartCheck.Failure("why");

        // Assert
        Check(!check.IsValid && check.Error == "why" && check.ValueColumns.Count == 0);
    }

    /// <summary>
    /// Tests PointKeepsItsValues.
    /// </summary>
    private static void PointKeepsItsValues()
    {
        // Setup
        double?[] values = [null, 1];

        // Act
        var point = new NotebookChartPoint(DateTime.UnixEpoch,
            values);

        // Assert
        Check(point.At == DateTime.UnixEpoch && ReferenceEquals(point.Values,
            values));
    }

    #endregion
}
