using System.Text.Json;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests reading files written in the shape RepoDB Studio writes.
/// </summary>
internal static class NotebookFileReaderTests
{
    #region Private Variables

    /// <summary>
    /// A file as the Studio writes it, without a format version.
    /// </summary>
    private const string StudioFile = """
        {
          "Logs": [ { "Time": "2026-01-01T00:00:00+00:00", "Level": "Information", "Message": "module" } ],
          "Processes": [],
          "Sections": [
            { "Language": "Header", "Title": "Sales", "Description": "About", "IsCollapsed": true },
            { "Language": "Imports", "Script": "using System;" },
            { "Language": "Sql", "Name": "Orders", "Script": "SELECT 1", "ConnectionId": "11111111-1111-1111-1111-111111111111",
              "ElapsedSeconds": 1.5, "EditorHeight": 100, "TableHeight": 200, "ResultStatus": "1 row(s)",
              "Logs": [ { "Level": "Warning", "Message": "careful", "Time": "2026-01-01T00:00:00+00:00" } ],
              "Results": [ { "Columns": [ { "Name": "Id", "TypeName": "int" } ], "IsTruncated": false, "Rows": [ [ "1" ] ], "Title": "" } ] },
            { "Language": "CSharp", "Script": "return 1;", "Output": "Returned: 1" },
            { "Language": "Chart", "ChartSource": "CSharp", "Script": "SELECT 2", "ChartCode": "Chart();", "ChartType": "Bars",
              "ChartBucket": "1h", "ChartColors": { "Total": "#ff0000" },
              "Results": [ { "Columns": [ { "Name": "Name", "TypeName": "string" }, { "Name": "Total", "TypeName": "int" } ],
                "Rows": [ [ "a", "1" ] ] } ] }
          ],
          "Traces": []
        }
        """;

    #endregion

    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        RunSectionTests();
        RunRestoreTests();
        RunDefaultTests();
        RunRejectionTests();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs the loading of each section kind.
    /// </summary>
    private static void RunSectionTests()
    {
        LoadsEveryKind();
        LoadsTheHeader();
        LoadsTheImports();
        LoadsASqlSection();
        LoadsACSharpSection();
        LoadsAChart();
        LoadsTheChartColors();
    }

    /// <summary>
    /// Runs the restoring of state.
    /// </summary>
    private static void RunRestoreTests()
    {
        RestoresTheLayout();
        RestoresTheRun();
        RestoresTheResults();
        RestoresTheChartData();
        RestoresTheImportCheck();
        RestoresTheHostData();
        LoadedSectionsHaveNoRunState();
    }

    /// <summary>
    /// Runs the completion of missing values.
    /// </summary>
    private static void RunDefaultTests()
    {
        CompletesAnEmptyFile();
        CompletesNullCollections();
        CompletesNullTexts();
        CompletesAnEmptySection();
        SkipsNullEntries();
    }

    /// <summary>
    /// Runs rejected files.
    /// </summary>
    private static void RunRejectionTests()
    {
        RejectsANewerFormat();
        RejectsAnUnknownKind();
        RejectsAnUnknownChartSource();
        RejectsAnUnknownLogLevel();
    }

    /// <summary>
    /// Reads a file written as text.
    /// </summary>
    /// <param name="json">The text.</param>
    /// <returns>The notebook.</returns>
    private static NotebookDocument Read(string json) => NotebookFileReader.Read(JsonSerializer.Deserialize(json,
        NotebookFileJsonContext.Default.NotebookFileModel)!);

    /// <summary>
    /// Reads a section of the Studio file.
    /// </summary>
    /// <typeparam name="T">The section type.</typeparam>
    /// <param name="index">The position.</param>
    /// <returns>The section.</returns>
    private static T Section<T>(int index)
        where T : NotebookSection => (T)Read(StudioFile).Sections[index];

    /// <summary>
    /// Tests LoadsEveryKind.
    /// </summary>
    private static void LoadsEveryKind()
    {
        // Act
        var document = Read(StudioFile);

        // Assert
        Check(string.Join(',',
            document.Sections
                .Select(section => section.Type)) == "Header,Imports,Code,Code,Chart");
    }

    /// <summary>
    /// Tests LoadsTheHeader.
    /// </summary>
    private static void LoadsTheHeader()
    {
        // Act
        var header = Section<NotebookHeaderSection>(0);

        // Assert
        Check(header is { Title: "Sales", Description: "About", IsCollapsed: true });
    }

    /// <summary>
    /// Tests LoadsTheImports.
    /// </summary>
    private static void LoadsTheImports()
    {
        // Act
        var imports = Section<NotebookImportsSection>(1);

        // Assert
        Check(imports.Script == "using System;");
    }

    /// <summary>
    /// Tests LoadsASqlSection.
    /// </summary>
    private static void LoadsASqlSection()
    {
        // Act
        var section = Section<NotebookCodeSection>(2);

        // Assert
        Check(section is { Language: NotebookCodeLanguage.Sql, Name: "Orders", Script: "SELECT 1" });
        Check(section.ConnectionId == Guid.Parse("11111111-1111-1111-1111-111111111111"));
    }

    /// <summary>
    /// Tests LoadsACSharpSection.
    /// </summary>
    private static void LoadsACSharpSection()
    {
        // Act
        var section = Section<NotebookCodeSection>(3);

        // Assert
        Check(section is { Language: NotebookCodeLanguage.CSharp, Script: "return 1;", Output: "Returned: 1", Name: "" });
    }

    /// <summary>
    /// Tests LoadsAChart.
    /// </summary>
    private static void LoadsAChart()
    {
        // Act
        var chart = Section<NotebookChartSection>(4);

        // Assert
        Check(chart is { Source: NotebookCodeLanguage.CSharp, SqlScript: "SELECT 2", CSharpScript: "Chart();" });
        Check(chart is { ChartType: NotebookChartType.Bars, Bucket: NotebookChartBucket.OneHour });
    }

    /// <summary>
    /// Tests LoadsTheChartColors.
    /// </summary>
    private static void LoadsTheChartColors()
    {
        // Act
        var chart = Section<NotebookChartSection>(4);

        // Assert
        Check(chart.Colors["Total"] == "#FF0000");
    }

    /// <summary>
    /// Tests RestoresTheLayout.
    /// </summary>
    private static void RestoresTheLayout()
    {
        // Act
        var section = Section<NotebookCodeSection>(2);

        // Assert
        Check(section.EditorHeight == 100 && section.TableHeight == 200);
    }

    /// <summary>
    /// Tests RestoresTheRun.
    /// </summary>
    private static void RestoresTheRun()
    {
        // Act
        var section = Section<NotebookCodeSection>(2);

        // Assert
        Check(section.Elapsed.TotalSeconds == 1.5);
        Check(section.Log.Entries
            .Single() is { Level: NotebookLogLevel.Warning, Message: "careful" });
    }

    /// <summary>
    /// Tests RestoresTheResults.
    /// </summary>
    private static void RestoresTheResults()
    {
        // Act
        var section = Section<NotebookCodeSection>(2);

        // Assert
        Check(section.ResultStatus == "1 row(s)" && section.Results
            .Single().Rows[0][0] == "1");
    }

    /// <summary>
    /// Tests RestoresTheChartData.
    /// </summary>
    private static void RestoresTheChartData()
    {
        // Act
        var chart = Section<NotebookChartSection>(4);

        // Assert
        Check(chart.Data is { Series.Count: 1 } && chart.Data.Series[0].Color == "#FF0000");
    }

    /// <summary>
    /// Tests RestoresTheImportCheck.
    /// </summary>
    private static void RestoresTheImportCheck()
    {
        // Act
        var imports = Section<NotebookImportsSection>(1);

        // Assert
        Check(imports.Check is { IsValid: true });
    }

    /// <summary>
    /// Tests RestoresTheHostData.
    /// </summary>
    private static void RestoresTheHostData()
    {
        // Act
        var document = Read(StudioFile);

        // Assert
        Check(string.Join(',',
            document.HostData.Keys
                .Order(StringComparer.Ordinal)) == "Logs,Processes,Traces");
    }

    /// <summary>
    /// Tests LoadedSectionsHaveNoRunState.
    /// </summary>
    private static void LoadedSectionsHaveNoRunState()
    {
        // Act
        var section = Section<NotebookCodeSection>(2);

        // Assert
        Check(section.State == NotebookRunState.Pending);
    }

    /// <summary>
    /// Tests CompletesAnEmptyFile.
    /// </summary>
    private static void CompletesAnEmptyFile()
    {
        // Act
        var document = Read("{}");

        // Assert
        Check(document.Sections.Count == 0 && document.HostData.Count == 0);
    }

    /// <summary>
    /// Tests CompletesNullCollections.
    /// </summary>
    private static void CompletesNullCollections()
    {
        // Act
        var empty = Read("{\"Sections\":null}");
        var document = Read("{\"Sections\":[{\"Language\":\"Chart\",\"ChartColors\":null,\"Logs\":null,\"Results\":null}]}");

        // Assert
        Check(empty.Sections.Count == 0);
        Check(document.Sections
            .Single() is NotebookChartSection { Colors.Count: 0, Results.Count: 0, Log.Entries.Count: 0 });
    }

    /// <summary>
    /// Tests CompletesNullTexts.
    /// </summary>
    private static void CompletesNullTexts()
    {
        // Setup
        const string json = """
            { "Sections": [
              { "Language": "Header", "Title": null, "Description": null },
              { "Language": "Imports", "Script": null },
              { "Language": "Sql", "Script": null, "Name": null, "Output": null, "ResultStatus": null,
                "Logs": [ { "Level": "Debug", "Message": null } ] },
              { "Language": "Chart", "Script": null, "ChartCode": null, "ChartType": null, "ChartBucket": null }
            ] }
            """;

        // Act
        var document = Read(json);

        // Assert
        Check(document.Sections[0] is NotebookHeaderSection { Title: "", Description: "" });
        Check(document.Sections[2] is NotebookCodeSection { Script: "", Name: "SQLSection1", Output: "", ResultStatus: "" });
        Check(document.Sections[3] is NotebookChartSection { SqlScript: "", CSharpScript: "" });
    }

    /// <summary>
    /// Tests CompletesAnEmptySection.
    /// </summary>
    private static void CompletesAnEmptySection()
    {
        // Act
        var document = Read("{\"Sections\":[{}]}");

        // Assert
        Check(document.Sections
            .Single() is NotebookCodeSection { Language: NotebookCodeLanguage.Sql, Name: "SQLSection1" });
    }

    /// <summary>
    /// Tests SkipsNullEntries.
    /// </summary>
    private static void SkipsNullEntries()
    {
        // Act
        var document = Read("{\"Sections\":[null,{\"Logs\":[null],\"Results\":[null]}]}");

        // Assert
        Check(document.Sections
            .Single() is NotebookCodeSection { Results.Count: 0, Log.Entries.Count: 0 });
    }

    /// <summary>
    /// Tests RejectsANewerFormat.
    /// </summary>
    private static void RejectsANewerFormat()
    {
        // Act
        Action operation = () => Read("{\"FormatVersion\":2}");

        // Assert
        Throws<InvalidDataException>(operation);
    }

    /// <summary>
    /// Tests RejectsAnUnknownKind.
    /// </summary>
    private static void RejectsAnUnknownKind()
    {
        // Act
        Action operation = () => Read("{\"Sections\":[{\"Language\":99}]}");

        // Assert
        Throws<InvalidDataException>(operation);
    }

    /// <summary>
    /// Tests RejectsAnUnknownChartSource.
    /// </summary>
    private static void RejectsAnUnknownChartSource()
    {
        // Act
        Action operation = () => Read("{\"Sections\":[{\"Language\":\"Chart\",\"ChartSource\":99}]}");

        // Assert
        Throws<InvalidDataException>(operation);
    }

    /// <summary>
    /// Tests RejectsAnUnknownLogLevel.
    /// </summary>
    private static void RejectsAnUnknownLogLevel()
    {
        // Act
        Action operation = () => Read("{\"Sections\":[{\"Logs\":[{\"Level\":99}]}]}");

        // Assert
        Throws<InvalidDataException>(operation);
    }

    #endregion
}
