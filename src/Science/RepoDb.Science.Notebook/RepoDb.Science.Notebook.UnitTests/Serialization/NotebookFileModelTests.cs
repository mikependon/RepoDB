using System.Text.Json;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the file models, their JSON metadata and the table mapper.
/// </summary>
internal static class NotebookFileModelTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        FileModelHasDefaults();
        SectionModelHasChartDefaults();
        SectionModelHasEmptyTexts();
        SectionModelHasEmptyCollections();
        LogModelHasDefaults();
        TableModelHasDefaults();
        ColumnModelHasDefaults();
        ContextWritesEnumsAsTexts();
        ContextReadsExtensionData();
        MapperWritesATable();
        MapperReadsATable();
        MapperCompletesAnEmptyTable();
        MapperSkipsNullEntries();
        MapperCompletesColumns();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Tests FileModelHasDefaults.
    /// </summary>
    private static void FileModelHasDefaults()
    {
        // Act
        var model = new NotebookFileModel();

        // Assert
        Check(model.FormatVersion == NotebookDocument.FormatVersion && model.Sections!.Count == 0 && model.HostData is null);
    }

    /// <summary>
    /// Tests SectionModelHasChartDefaults.
    /// </summary>
    private static void SectionModelHasChartDefaults()
    {
        // Act
        var model = new NotebookSectionFileModel();

        // Assert
        Check(model.ChartBucket == "1min" && model.ChartType == "Lines (Stacked)" && model.ChartSource == NotebookCodeLanguage.Sql);
        Check(model.Language == NotebookFileSectionKind.Sql && model.ConnectionId is null);
    }

    /// <summary>
    /// Tests SectionModelHasEmptyTexts.
    /// </summary>
    private static void SectionModelHasEmptyTexts()
    {
        // Act
        var model = new NotebookSectionFileModel();

        // Assert
        Check(new[] { model.ChartCode, model.Description, model.Name, model.Output, model.ResultStatus, model.Script, model.Title }
            .All(text => text == ""));
        Check(model is { EditorHeight: 0, TableHeight: 0, ElapsedSeconds: 0, IsCollapsed: false });
    }

    /// <summary>
    /// Tests SectionModelHasEmptyCollections.
    /// </summary>
    private static void SectionModelHasEmptyCollections()
    {
        // Act
        var model = new NotebookSectionFileModel();

        // Assert
        Check(model.ChartColors!.Count == 0 && model.Logs!.Count == 0 && model.Results!.Count == 0);
    }

    /// <summary>
    /// Tests LogModelHasDefaults.
    /// </summary>
    private static void LogModelHasDefaults()
    {
        // Act
        var model = new NotebookLogFileModel();

        // Assert
        Check(model.Level == NotebookLogLevel.Debug && model.Message == "" && model.Time == default);
    }

    /// <summary>
    /// Tests TableModelHasDefaults.
    /// </summary>
    private static void TableModelHasDefaults()
    {
        // Act
        var model = new NotebookTableFileModel();

        // Assert
        Check(model.Columns!.Count == 0 && model.Rows!.Count == 0 && model.Title == "" && !model.IsTruncated);
    }

    /// <summary>
    /// Tests ColumnModelHasDefaults.
    /// </summary>
    private static void ColumnModelHasDefaults()
    {
        // Act
        var model = new NotebookColumnFileModel();

        // Assert
        Check(model.Name == "" && model.TypeName == "string");
    }

    /// <summary>
    /// Tests ContextWritesEnumsAsTexts.
    /// </summary>
    private static void ContextWritesEnumsAsTexts()
    {
        // Setup
        var model = new NotebookFileModel { Sections = [new() { Language = NotebookFileSectionKind.Chart }] };

        // Act
        var json = JsonSerializer.Serialize(model,
            NotebookFileJsonContext.Default.NotebookFileModel);

        // Assert
        Check(json.Contains("\"Language\": \"Chart\"",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests ContextReadsExtensionData.
    /// </summary>
    private static void ContextReadsExtensionData()
    {
        // Act
        var model = JsonSerializer.Deserialize("{\"Traces\":[1]}",
            NotebookFileJsonContext.Default.NotebookFileModel)!;

        // Assert
        Check(model.HostData!["Traces"].GetArrayLength() == 1);
    }

    /// <summary>
    /// Tests MapperWritesATable.
    /// </summary>
    private static void MapperWritesATable()
    {
        // Setup
        var table = new NotebookTable([new("Id",
                "int")],
            [["1"]],
            true,
            "T");

        // Act
        var model = NotebookTableMapper.ToModel(table);

        // Assert
        Check(model.Columns![0] is { Name: "Id", TypeName: "int" } && model.Rows![0]![0] == "1");
        Check(model is { IsTruncated: true, Title: "T" });
    }

    /// <summary>
    /// Tests MapperReadsATable.
    /// </summary>
    private static void MapperReadsATable()
    {
        // Setup
        var model = new NotebookTableFileModel
        {
            Columns = [new() { Name = "Id", TypeName = "int" }],
            Rows = [["1"]],
            IsTruncated = true,
            Title = "T"
        };

        // Act
        var table = NotebookTableMapper.FromModel(model);

        // Assert
        Check(table.Columns[0].TypeName == "int" && table.Rows[0][0] == "1" && table is { IsTruncated: true, Title: "T" });
    }

    /// <summary>
    /// Tests MapperCompletesAnEmptyTable.
    /// </summary>
    private static void MapperCompletesAnEmptyTable()
    {
        // Setup
        var model = new NotebookTableFileModel { Columns = null, Rows = null, Title = null };

        // Act
        var table = NotebookTableMapper.FromModel(model);

        // Assert
        Check(table.Columns.Count == 0 && table.Rows.Count == 0 && table.Title.Length == 0);
    }

    /// <summary>
    /// Tests MapperSkipsNullEntries.
    /// </summary>
    private static void MapperSkipsNullEntries()
    {
        // Setup
        var model = new NotebookTableFileModel { Columns = [null], Rows = [null] };

        // Act
        var table = NotebookTableMapper.FromModel(model);

        // Assert
        Check(table.Columns.Count == 0 && table.Rows.Count == 0);
    }

    /// <summary>
    /// Tests MapperCompletesColumns.
    /// </summary>
    private static void MapperCompletesColumns()
    {
        // Setup
        var model = new NotebookTableFileModel { Columns = [new() { Name = null, TypeName = null }] };

        // Act
        var column = NotebookTableMapper.FromModel(model).Columns
            .Single();

        // Assert
        Check(column.Name.Length == 0 && column.TypeName == "string");
    }

    #endregion
}
