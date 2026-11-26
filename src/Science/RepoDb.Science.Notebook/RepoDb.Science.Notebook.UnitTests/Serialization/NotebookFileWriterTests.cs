using System.Text.Json;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests writing a notebook to its file shape.
/// </summary>
internal static class NotebookFileWriterTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        RunSectionTests();
        RunStateTests();
        RunRoundTripTests();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs the writing of each section kind.
    /// </summary>
    private static void RunSectionTests()
    {
        WritesTheHeader();
        WritesTheImports();
        WritesASqlSection();
        WritesACSharpSection();
        WritesAChart();
    }

    /// <summary>
    /// Runs the writing of state.
    /// </summary>
    private static void RunStateTests()
    {
        WritesTheLayout();
        WritesTheRun();
        WritesTheResults();
        WritesTheHostData();
        LeavesOutReservedHostData();
        WritesAnIndependentCopy();
    }

    /// <summary>
    /// Runs round trips.
    /// </summary>
    private static void RunRoundTripTests()
    {
        RoundTripKeepsEverything();
    }

    /// <summary>
    /// Writes one section.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <returns>The saved section.</returns>
    private static NotebookSectionFileModel Write(NotebookSection section)
    {
        var document = new NotebookDocument();
        document.Sections.Add(section);
        return NotebookFileWriter.Write(document).Sections!
            .Single()!;
    }

    /// <summary>
    /// Creates a notebook with every kind of section and state.
    /// </summary>
    /// <returns>The notebook.</returns>
    private static NotebookDocument Everything()
    {
        var document = NotebookDocument.Create("Sales");
        var chart = new NotebookChartSection { SqlScript = "SELECT 2", ChartType = NotebookChartType.StackedBars, IsCollapsed = true };
        chart.SetColors(new Dictionary<string, string>(StringComparer.Ordinal) { ["Total"] = "#000000" });
        chart.RestoreResults("",
            [SampleTables.Timed()],
            "3 row(s)");
        document.Sections.Add(chart);
        document.HostData["Traces"] = JsonDocument.Parse("[1]").RootElement.Clone();
        return document;
    }

    /// <summary>
    /// Writes a notebook as text.
    /// </summary>
    /// <param name="document">The notebook.</param>
    /// <returns>The text.</returns>
    private static string Json(NotebookDocument document) => JsonSerializer.Serialize(NotebookFileWriter.Write(document),
        NotebookFileJsonContext.Default.NotebookFileModel);

    /// <summary>
    /// Tests WritesTheHeader.
    /// </summary>
    private static void WritesTheHeader()
    {
        // Act
        var model = Write(new NotebookHeaderSection { Title = "T", Description = "D" });

        // Assert
        Check(model is { Language: NotebookFileSectionKind.Header, Title: "T", Description: "D" });
    }

    /// <summary>
    /// Tests WritesTheImports.
    /// </summary>
    private static void WritesTheImports()
    {
        // Act
        var model = Write(new NotebookImportsSection { Script = "using A;" });

        // Assert
        Check(model is { Language: NotebookFileSectionKind.Imports, Script: "using A;" });
    }

    /// <summary>
    /// Tests WritesASqlSection.
    /// </summary>
    private static void WritesASqlSection()
    {
        // Setup
        var id = Guid.NewGuid();

        // Act
        var model = Write(new NotebookCodeSection { Script = "SELECT 1", ConnectionId = id });

        // Assert
        Check(model is { Language: NotebookFileSectionKind.Sql, Name: "SQLSection1", Script: "SELECT 1" });
        Check(model.ConnectionId == id);
    }

    /// <summary>
    /// Tests WritesACSharpSection.
    /// </summary>
    private static void WritesACSharpSection()
    {
        // Act
        var model = Write(new NotebookCodeSection { Language = NotebookCodeLanguage.CSharp, Script = "x" });

        // Assert
        Check(model is { Language: NotebookFileSectionKind.CSharp, Script: "x" });
    }

    /// <summary>
    /// Tests WritesAChart.
    /// </summary>
    private static void WritesAChart()
    {
        // Act
        var model = Write(new NotebookChartSection
        {
            Source = NotebookCodeLanguage.CSharp,
            SqlScript = "sql",
            CSharpScript = "code",
            ChartType = NotebookChartType.Lines,
            Bucket = NotebookChartBucket.OneDay
        });

        // Assert
        Check(model is { Language: NotebookFileSectionKind.Chart, ChartSource: NotebookCodeLanguage.CSharp, Script: "sql" });
        Check(model is { ChartCode: "code", ChartType: "Lines", ChartBucket: "1d" });
    }

    /// <summary>
    /// Tests WritesTheLayout.
    /// </summary>
    private static void WritesTheLayout()
    {
        // Act
        var model = Write(new NotebookCodeSection { IsCollapsed = true, EditorHeight = 10, TableHeight = 20 });

        // Assert
        Check(model is { IsCollapsed: true, EditorHeight: 10, TableHeight: 20 });
    }

    /// <summary>
    /// Tests WritesTheRun.
    /// </summary>
    private static void WritesTheRun()
    {
        // Setup
        var section = new NotebookCodeSection();
        section.RestoreRun(TimeSpan.FromSeconds(2),
            [new NotebookLogEntry(NotebookLogLevel.Error,
                "bad",
                DateTimeOffset.UnixEpoch)]);

        // Act
        var model = Write(section);

        // Assert
        Check(model.ElapsedSeconds == 2);
        Check(model.Logs!
            .Single() is { Level: NotebookLogLevel.Error, Message: "bad" });
    }

    /// <summary>
    /// Tests WritesTheResults.
    /// </summary>
    private static void WritesTheResults()
    {
        // Setup
        var section = new NotebookCodeSection();
        section.RestoreResults("out",
            [SampleTables.Categories()],
            "2 row(s)");

        // Act
        var model = Write(section);

        // Assert
        Check(model is { Output: "out", ResultStatus: "2 row(s)", Results.Count: 1 });
    }

    /// <summary>
    /// Tests WritesTheHostData.
    /// </summary>
    private static void WritesTheHostData()
    {
        // Act
        var model = NotebookFileWriter.Write(Everything());

        // Assert
        Check(model.HostData!["Traces"].GetArrayLength() == 1);
    }

    /// <summary>
    /// Tests LeavesOutReservedHostData.
    /// </summary>
    private static void LeavesOutReservedHostData()
    {
        // Setup
        var document = new NotebookDocument();
        document.HostData["Sections"] = default;
        document.HostData["FormatVersion"] = default;

        // Act
        var model = NotebookFileWriter.Write(document);

        // Assert
        Check(model.HostData!.Count == 0);
    }

    /// <summary>
    /// Tests WritesAnIndependentCopy.
    /// </summary>
    private static void WritesAnIndependentCopy()
    {
        // Setup
        var document = Everything();
        var model = NotebookFileWriter.Write(document);

        // Act
        document.Sections.Clear();
        document.HostData.Clear();

        // Assert
        Check(model.Sections!.Count == 5 && model.HostData!.Count == 1);
    }

    /// <summary>
    /// Tests RoundTripKeepsEverything.
    /// </summary>
    private static void RoundTripKeepsEverything()
    {
        // Setup
        var json = Json(Everything());

        // Act
        var loaded = NotebookFileReader.Read(JsonSerializer.Deserialize(json,
            NotebookFileJsonContext.Default.NotebookFileModel)!);

        // Assert
        Check(Json(loaded) == json);
    }

    #endregion
}
