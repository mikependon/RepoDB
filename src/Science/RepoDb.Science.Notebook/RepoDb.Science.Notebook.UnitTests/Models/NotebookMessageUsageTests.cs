namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests that a notebook writes the texts of its host.
/// </summary>
internal static class NotebookMessageUsageTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static async Task RunAsync()
    {
        RunDocumentTests();
        await RunFileTests();
        await RunSqlTests();
        await RunCSharpTests();
        await RunChartTests();
        RunImportTests();
        await RunRunTests();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs the document scenarios.
    /// </summary>
    private static void RunDocumentTests()
    {
        DocumentUsesTheDefaultMessages();
        DocumentKeepsItsMessages();
        DocumentRejectsNullMessages();
        CreateUsesTheMessages();
    }

    /// <summary>
    /// Runs the file scenarios.
    /// </summary>
    private static async Task RunFileTests()
    {
        OpenUsesTheMessages();
        OpenRejectsNullMessages();
        await OpenAsyncUsesTheMessages();
        await OpenAsyncRejectsNullMessages();
    }

    /// <summary>
    /// Runs the SQL scenarios.
    /// </summary>
    private static async Task RunSqlTests()
    {
        await SqlLinesComeFromTheHost();
        await LimitedRowsComeFromTheHost();
        await AffectedRowsComeFromTheHost();
        await MissingSqlPartsComeFromTheHost();
        await StoppedSectionComesFromTheHost();
    }

    /// <summary>
    /// Runs the C# scenarios.
    /// </summary>
    private static async Task RunCSharpTests()
    {
        await CSharpTextsComeFromTheHost();
        await CSharpProblemsComeFromTheHost();
    }

    /// <summary>
    /// Runs the chart scenarios.
    /// </summary>
    private static async Task RunChartTests()
    {
        await ChartLinesComeFromTheHost();
        await ChartProblemsComeFromTheHost();
        await MissingChartTablesComeFromTheHost();
    }

    /// <summary>
    /// Runs the import scenarios.
    /// </summary>
    private static void RunImportTests()
    {
        ImportProblemsComeFromTheHost();
        ImportSummariesComeFromTheHost();
        AttachingRewordsAnExistingCheck();
        AttachingKeepsASectionUnchecked();
        EmptyImportTextStaysAProblem();
    }

    /// <summary>
    /// Runs the run scenarios.
    /// </summary>
    private static async Task RunRunTests()
    {
        PlanUsesTheHostNames();
        await FailedRunUsesTheHostText();
        await StoppedRunUsesTheHostText();
        await EmptyTextsStillReportAFailedRun();
    }

    /// <summary>
    /// Adds a section to a notebook with tagged messages.
    /// </summary>
    /// <typeparam name="T">The section type.</typeparam>
    /// <param name="section">The section.</param>
    /// <returns>The section.</returns>
    private static T Tagged<T>(T section)
        where T : NotebookSection
    {
        new NotebookDocument(new TagMessages()).Sections.Add(section);
        return section;
    }

    /// <summary>
    /// Runs a tagged section.
    /// </summary>
    /// <typeparam name="T">The section type.</typeparam>
    /// <param name="section">The section.</param>
    /// <param name="context">The context.</param>
    /// <returns>The log lines and the status message.</returns>
    private static async Task<string> TextsOf<T>(T section,
        NotebookExecutionContext context)
        where T : NotebookExecutableSection
    {
        Tagged(section);
        await section.ExecuteAsync(context);
        return string.Join('|',
            section.Log.Entries
                .Select(entry => entry.Message)
                .Append(section.StatusMessage));
    }

    /// <summary>
    /// Runs a chart against tables.
    /// </summary>
    /// <param name="tables">The tables.</param>
    /// <returns>The texts.</returns>
    private static Task<string> ChartTexts(params NotebookTable[] tables) => TextsOf(new NotebookChartSection { SqlScript = "q" },
        new NotebookExecutionContext { SqlExecutor = new FakeSqlExecutor(tables) });

    /// <summary>
    /// Runs a C# section against an outcome.
    /// </summary>
    /// <param name="result">The outcome.</param>
    /// <returns>The texts.</returns>
    private static Task<string> CSharpTexts(NotebookScriptResult result) => TextsOf(new NotebookCodeSection { Language = NotebookCodeLanguage.CSharp },
        new NotebookExecutionContext { CSharpExecutor = new FakeCSharpExecutor(result) });

    /// <summary>
    /// Creates a notebook whose SQL section fails in a run.
    /// </summary>
    /// <param name="messages">The messages.</param>
    /// <returns>The notebook.</returns>
    private static NotebookDocument Failing(NotebookMessages messages) => NotebookDocument.Create(messages: messages);

    /// <summary>
    /// Saves a notebook with imports.
    /// </summary>
    /// <param name="folder">The folder.</param>
    /// <returns>The file.</returns>
    private static string Saved(TestFolder folder)
    {
        var path = folder.FileOf("a.rdbnb");
        NotebookDocument.Create(imports: "using A;")
            .Save(path);
        return path;
    }

    /// <summary>
    /// Gets the summary of the imports of a notebook.
    /// </summary>
    /// <param name="document">The notebook.</param>
    /// <returns>The summary.</returns>
    private static string? SummaryOf(NotebookDocument document) => ((NotebookImportsSection)document.Sections[1]).Check?.Summary;

    /// <summary>
    /// Tests DocumentUsesTheDefaultMessages.
    /// </summary>
    private static void DocumentUsesTheDefaultMessages() => Check(ReferenceEquals(new NotebookDocument().Messages,
        NotebookMessages.Default));

    /// <summary>
    /// Tests DocumentKeepsItsMessages.
    /// </summary>
    private static void DocumentKeepsItsMessages()
    {
        // Setup
        var messages = new TagMessages();

        // Act
        var document = new NotebookDocument(messages);

        // Assert
        Check(ReferenceEquals(document.Messages,
            messages));
    }

    /// <summary>
    /// Tests DocumentRejectsNullMessages.
    /// </summary>
    private static void DocumentRejectsNullMessages()
    {
        // Act
        Action operation = () => _ = new NotebookDocument(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests CreateUsesTheMessages.
    /// </summary>
    private static void CreateUsesTheMessages()
    {
        // Act
        var document = NotebookDocument.Create(messages: new TagMessages());

        // Assert
        Check(((NotebookHeaderSection)document.Sections[0]).Description == "description");
    }

    /// <summary>
    /// Tests OpenUsesTheMessages.
    /// </summary>
    private static void OpenUsesTheMessages()
    {
        // Setup
        using var folder = new TestFolder();
        var path = Saved(folder);

        // Act
        var document = NotebookDocument.Open(path,
            new TagMessages());

        // Assert
        Check(SummaryOf(document) == "imports-valid:1" && !document.HasChanges);
    }

    /// <summary>
    /// Tests OpenRejectsNullMessages.
    /// </summary>
    private static void OpenRejectsNullMessages()
    {
        // Act
        Action operation = () => NotebookDocument.Open("a.rdbnb",
            null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests OpenAsyncUsesTheMessages.
    /// </summary>
    private static async Task OpenAsyncUsesTheMessages()
    {
        // Setup
        using var folder = new TestFolder();
        var path = Saved(folder);

        // Act
        var document = await NotebookDocument.OpenAsync(path,
            new TagMessages());

        // Assert
        Check(SummaryOf(document) == "imports-valid:1");
    }

    /// <summary>
    /// Tests OpenAsyncRejectsNullMessages.
    /// </summary>
    private static async Task OpenAsyncRejectsNullMessages()
    {
        // Act/Assert
        await ThrowsAsync<ArgumentNullException>(() => NotebookDocument.OpenAsync("a.rdbnb",
            (NotebookMessages)null!));
    }

    /// <summary>
    /// Tests SqlLinesComeFromTheHost.
    /// </summary>
    private static async Task SqlLinesComeFromTheHost()
    {
        // Setup
        var id = Guid.NewGuid();

        // Act
        var texts = await TextsOf(new NotebookCodeSection { Script = "q", ConnectionId = id },
            new NotebookExecutionContext { SqlExecutor = new FakeSqlExecutor(SampleTables.Categories()) });

        // Assert
        Check(texts == $"sql-started:{id}|sql-completed:rows:2|rows:2");
    }

    /// <summary>
    /// Tests LimitedRowsComeFromTheHost.
    /// </summary>
    private static async Task LimitedRowsComeFromTheHost()
    {
        // Act
        var texts = await TextsOf(new NotebookCodeSection { Script = "q" },
            new NotebookExecutionContext { SqlExecutor = new FakeSqlExecutor(SampleTables.Categories()), RowLimit = 1 });

        // Assert
        Check(texts == "sql-started:|sql-limited:1|rows:1");
    }

    /// <summary>
    /// Tests AffectedRowsComeFromTheHost.
    /// </summary>
    private static async Task AffectedRowsComeFromTheHost()
    {
        // Act
        var texts = await TextsOf(new NotebookCodeSection { Script = "q" },
            new NotebookExecutionContext { SqlExecutor = FakeSqlExecutor.Answering(_ => new NotebookSqlResult { RowsAffected = 4 }) });

        // Assert
        Check(texts.EndsWith("affected:4",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests MissingSqlPartsComeFromTheHost.
    /// </summary>
    private static async Task MissingSqlPartsComeFromTheHost()
    {
        // Act
        var executor = await TextsOf(new NotebookCodeSection(),
            new NotebookExecutionContext());
        var script = await TextsOf(new NotebookCodeSection(),
            new NotebookExecutionContext { SqlExecutor = new FakeSqlExecutor() });

        // Assert
        Check(executor == "sql-executor-missing|sql-executor-missing");
        Check(script == "sql-script-missing|sql-script-missing");
    }

    /// <summary>
    /// Tests StoppedSectionComesFromTheHost.
    /// </summary>
    private static async Task StoppedSectionComesFromTheHost()
    {
        // Setup
        var section = Tagged(new NotebookCodeSection());

        // Act
        await ThrowsAsync<OperationCanceledException>(() => section.ExecuteAsync(new NotebookExecutionContext(),
            new CancellationToken(true)));

        // Assert
        Check(section.StatusMessage == "stopped:Code" && section.Log.Entries
            .Single().Message == "stopped:Code");
    }

    /// <summary>
    /// Tests CSharpTextsComeFromTheHost.
    /// </summary>
    private static async Task CSharpTextsComeFromTheHost()
    {
        // Setup
        var section = Tagged(new NotebookCodeSection { Language = NotebookCodeLanguage.CSharp });
        var result = new NotebookScriptResult { Succeeded = true, ReturnValue = "1" };

        // Act
        await section.ExecuteAsync(new NotebookExecutionContext { CSharpExecutor = new FakeCSharpExecutor(result) });

        // Assert
        Check(section.Output == "returned:1" && section.Log.Entries
            .Single().Message == "csharp-succeeded");
    }

    /// <summary>
    /// Tests CSharpProblemsComeFromTheHost.
    /// </summary>
    private static async Task CSharpProblemsComeFromTheHost()
    {
        // Act
        var missing = await TextsOf(new NotebookCodeSection { Language = NotebookCodeLanguage.CSharp },
            new NotebookExecutionContext());
        var unavailable = await CSharpTexts(new NotebookScriptResult { IsUnavailable = true });
        var failed = await CSharpTexts(new NotebookScriptResult());

        // Assert
        Check(missing == "csharp-executor-missing|csharp-executor-missing");
        Check(unavailable == "csharp-unavailable|csharp-unavailable" && failed == "csharp-failed|csharp-failed");
    }

    /// <summary>
    /// Tests ChartLinesComeFromTheHost.
    /// </summary>
    private static async Task ChartLinesComeFromTheHost()
    {
        // Setup
        var table = SampleTables.Create(["Name:string", "Total:int", "Note:string"],
            ["a", "1", "x"]);

        // Act
        var texts = await ChartTexts(table);

        // Assert
        Check(texts.Contains("charted:1:1|label-column:Name|skipped:Note",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests ChartProblemsComeFromTheHost.
    /// </summary>
    private static async Task ChartProblemsComeFromTheHost()
    {
        // Act
        var rows = await ChartTexts(SampleTables.Create(["a:int", "b:int"]));
        var columns = await ChartTexts(SampleTables.Create(["a:int"],
            ["1"]));
        var times = await ChartTexts(SampleTables.Create(["a:datetime", "b:int"],
            [null, "1"]));
        var numbers = await ChartTexts(SampleTables.Create(["a:string", "b:string"],
            ["x", "y"]));

        // Assert
        Check(rows.EndsWith("chart-no-rows",
            StringComparison.Ordinal) && columns.EndsWith("chart-few-columns",
                StringComparison.Ordinal));
        Check(times.EndsWith("chart-no-timestamp",
            StringComparison.Ordinal) && numbers.EndsWith("chart-no-numbers",
                StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests MissingChartTablesComeFromTheHost.
    /// </summary>
    private static async Task MissingChartTablesComeFromTheHost()
    {
        // Act
        var sql = await ChartTexts();
        var csharp = await TextsOf(new NotebookChartSection { Source = NotebookCodeLanguage.CSharp },
            new NotebookExecutionContext { CSharpExecutor = new FakeCSharpExecutor(new NotebookScriptResult { Succeeded = true }) });

        // Assert
        Check(sql.EndsWith("sql-chart-missing",
            StringComparison.Ordinal));
        Check(csharp.EndsWith("csharp-chart-missing",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests ImportProblemsComeFromTheHost.
    /// </summary>
    private static void ImportProblemsComeFromTheHost()
    {
        // Setup
        var script = "x;\nusing;\nusing var a;\nusing 9;\nusing J =;\nusing A\nusing B; using B;";

        // Act
        var check = NotebookImportValidator.Check(script,
            new TagMessages());

        // Assert
        Check(string.Join('|',
            check.Errors
                .Select(issue => issue.Text)) == "1>import-code|2>import-target|3>import-statement|4>import-name:9|5>import-alias:J = |6>import-semicolon:using A");
        Check(check.Warnings
            .Single().Text == "7>import-duplicate:using B;");
    }

    /// <summary>
    /// Tests ImportSummariesComeFromTheHost.
    /// </summary>
    private static void ImportSummariesComeFromTheHost()
    {
        // Setup
        var messages = new TagMessages();

        // Assert
        Check(NotebookImportValidator.Check("",
            messages).Summary == "no-imports");
        Check(NotebookImportValidator.Check("x;",
            messages).Summary == "import-problems:1");
    }

    /// <summary>
    /// Tests AttachingRewordsAnExistingCheck.
    /// </summary>
    private static void AttachingRewordsAnExistingCheck()
    {
        // Setup
        var section = new NotebookImportsSection { Script = "using A;" };
        section.RestoreCheck();

        // Act
        Tagged(section);

        // Assert
        Check(section.Check!.Summary == "imports-valid:1");
    }

    /// <summary>
    /// Tests AttachingKeepsASectionUnchecked.
    /// </summary>
    private static void AttachingKeepsASectionUnchecked()
    {
        // Act
        var section = Tagged(new NotebookImportsSection { Script = "using A;" });

        // Assert
        Check(section.Check is null);
    }

    /// <summary>
    /// Tests EmptyImportTextStaysAProblem.
    /// </summary>
    private static void EmptyImportTextStaysAProblem()
    {
        // Act
        var check = NotebookImportValidator.Check("x;",
            new SilentMessages());

        // Assert
        Check(!check.IsValid && check.Entries.Count == 0);
    }

    /// <summary>
    /// Tests PlanUsesTheHostNames.
    /// </summary>
    private static void PlanUsesTheHostNames()
    {
        // Setup
        var document = NotebookDocument.Create(messages: new TagMessages());
        document.Sections.Add(new NotebookChartSection());

        // Act
        var names = document.GetRunPlan()
            .Select(step => step.Name);

        // Assert
        Check(string.Join('|',
            names) == "imports-step|SQLSection1|csharp-step:3|chart-step:4");
    }

    /// <summary>
    /// Tests FailedRunUsesTheHostText.
    /// </summary>
    private static async Task FailedRunUsesTheHostText()
    {
        // Act
        var result = await Failing(new TagMessages())
            .ExecuteAsync(new NotebookExecutionContext());

        // Assert
        Check(result.FailureMessage == "run-failed:SQLSection1");
    }

    /// <summary>
    /// Tests StoppedRunUsesTheHostText.
    /// </summary>
    private static async Task StoppedRunUsesTheHostText()
    {
        // Act
        var result = await Failing(new TagMessages())
            .ExecuteAsync(new NotebookExecutionContext(),
                cancellationToken: new CancellationToken(true));

        // Assert
        Check(result.FailureMessage == "run-stopped");
    }

    /// <summary>
    /// Tests EmptyTextsStillReportAFailedRun.
    /// </summary>
    private static async Task EmptyTextsStillReportAFailedRun()
    {
        // Act
        var result = await Failing(new SilentMessages())
            .ExecuteAsync(new NotebookExecutionContext());

        // Assert
        Check(!result.Succeeded && result.FailureMessage.Length == 0);
        Check(result.Items[^1].State == NotebookRunState.Skipped);
    }

    #endregion
}
