namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests how script sections run C#.
/// </summary>
internal static class NotebookScriptSectionCSharpTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static async Task RunAsync()
    {
        await RunIsSkippedWithoutAnExecutor();
        await RequestCarriesTheCode();
        await RequestCarriesTheNotebookData();
        await RequestOfALooseSectionIsEmpty();
        await ScriptLogsAreKept();
        await UnavailableScriptIsSkipped();
        await UnavailableScriptHasADefaultReason();
        await OutputAndReturnValueAreJoined();
        await ReturnValueAloneIsTheOutput();
        await OutputIsTrimmed();
        await TablesAreShownAndLimited();
        await RunWithoutTablesHasNoStatus();
        await FailedScriptKeepsItsError();
        await FailedScriptHasADefaultError();
        await SuccessfulScriptIsLogged();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs a C# section with an outcome.
    /// </summary>
    /// <param name="result">The outcome.</param>
    /// <returns>The section.</returns>
    private static async Task<NotebookCodeSection> Ran(NotebookScriptResult result)
    {
        var section = new NotebookCodeSection { Language = NotebookCodeLanguage.CSharp, Script = "return 1;" };
        await section.ExecuteAsync(new NotebookExecutionContext
        {
            CSharpExecutor = new FakeCSharpExecutor(result),
            RowLimit = 1,
            OutputLimit = 1
        });
        return section;
    }

    /// <summary>
    /// Tests RunIsSkippedWithoutAnExecutor.
    /// </summary>
    private static async Task RunIsSkippedWithoutAnExecutor()
    {
        // Setup
        var section = new NotebookCodeSection { Language = NotebookCodeLanguage.CSharp };

        // Act
        await section.ExecuteAsync(new NotebookExecutionContext());

        // Assert
        Check(section.State == NotebookRunState.Skipped && section.StatusMessage == "C# scripts cannot be run yet.");
        Check(section.Log.Entries
            .Single().Level == NotebookLogLevel.Warning);
    }

    /// <summary>
    /// Tests RequestCarriesTheCode.
    /// </summary>
    private static async Task RequestCarriesTheCode()
    {
        // Setup
        var executor = new FakeCSharpExecutor(new NotebookScriptResult { Succeeded = true });
        var section = new NotebookCodeSection { Language = NotebookCodeLanguage.CSharp, Script = "return 1;" };

        // Act
        await section.ExecuteAsync(new NotebookExecutionContext { CSharpExecutor = executor, RowLimit = 4 });

        // Assert
        Check(executor.Requests
            .Single() is { Code: "return 1;", RowLimit: 4 });
    }

    /// <summary>
    /// Tests RequestCarriesTheNotebookData.
    /// </summary>
    private static async Task RequestCarriesTheNotebookData()
    {
        // Setup
        var executor = new FakeCSharpExecutor(new NotebookScriptResult { Succeeded = true });
        var document = NotebookDocument.Create(imports: "using A;");
        var sql = document.Sections
            .OfType<NotebookCodeSection>()
            .First();
        sql.RestoreResults("",
            [SampleTables.Categories()],
            "");

        // Act
        await document.Sections[3].ExecuteSection(new NotebookExecutionContext { CSharpExecutor = executor });

        // Assert
        Check(executor.Requests
            .Single().Imports == "using A;");
        Check(executor.Requests
            .Single().SqlResults["sqlsection1"].Count == 1);
    }

    /// <summary>
    /// Runs a section of a collection.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <param name="context">The context.</param>
    private static Task ExecuteSection(this NotebookSection section,
        NotebookExecutionContext context) => ((NotebookExecutableSection)section).ExecuteAsync(context);

    /// <summary>
    /// Tests RequestOfALooseSectionIsEmpty.
    /// </summary>
    private static async Task RequestOfALooseSectionIsEmpty()
    {
        // Setup
        var executor = new FakeCSharpExecutor(new NotebookScriptResult { Succeeded = true });
        var section = new NotebookCodeSection { Language = NotebookCodeLanguage.CSharp };

        // Act
        await section.ExecuteAsync(new NotebookExecutionContext { CSharpExecutor = executor });

        // Assert
        Check(executor.Requests
            .Single() is { Imports.Length: 0, SqlResults.Count: 0 });
    }

    /// <summary>
    /// Tests ScriptLogsAreKept.
    /// </summary>
    private static async Task ScriptLogsAreKept()
    {
        // Setup
        var entry = new NotebookLogEntry(NotebookLogLevel.Warning,
            "Compiler",
            DateTimeOffset.UnixEpoch);

        // Act
        var section = await Ran(new NotebookScriptResult { Succeeded = true, Logs = [entry] });

        // Assert
        Check(section.Log.Entries[0] is { Level: NotebookLogLevel.Warning, Message: "Compiler" });
    }

    /// <summary>
    /// Tests UnavailableScriptIsSkipped.
    /// </summary>
    private static async Task UnavailableScriptIsSkipped()
    {
        // Act
        var section = await Ran(new NotebookScriptResult { IsUnavailable = true, Error = "Missing" });

        // Assert
        Check(section.State == NotebookRunState.Skipped && section.StatusMessage == "Missing");
    }

    /// <summary>
    /// Tests UnavailableScriptHasADefaultReason.
    /// </summary>
    private static async Task UnavailableScriptHasADefaultReason()
    {
        // Act
        var section = await Ran(new NotebookScriptResult { IsUnavailable = true });

        // Assert
        Check(section.StatusMessage == "The C# executor is not available.");
    }

    /// <summary>
    /// Tests OutputAndReturnValueAreJoined.
    /// </summary>
    private static async Task OutputAndReturnValueAreJoined()
    {
        // Act
        var section = await Ran(new NotebookScriptResult { Succeeded = true, Output = "hello\n", ReturnValue = "1" });

        // Assert
        Check(section.Output == "hello" + Environment.NewLine + "Returned: 1");
    }

    /// <summary>
    /// Tests ReturnValueAloneIsTheOutput.
    /// </summary>
    private static async Task ReturnValueAloneIsTheOutput()
    {
        // Act
        var section = await Ran(new NotebookScriptResult { Succeeded = true, ReturnValue = "1" });

        // Assert
        Check(section.Output == "Returned: 1");
    }

    /// <summary>
    /// Tests OutputIsTrimmed.
    /// </summary>
    private static async Task OutputIsTrimmed()
    {
        // Act
        var section = await Ran(new NotebookScriptResult { Succeeded = true, Output = "a\nb" });

        // Assert
        Check(section.Output.StartsWith("... 1 earlier line not shown",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests TablesAreShownAndLimited.
    /// </summary>
    private static async Task TablesAreShownAndLimited()
    {
        // Act
        var section = await Ran(new NotebookScriptResult { Succeeded = true, Tables = [SampleTables.Categories()] });

        // Assert
        Check(section.Results
            .Single().Rows.Count == 1 && section.ResultStatus == "1 row(s)");
    }

    /// <summary>
    /// Tests RunWithoutTablesHasNoStatus.
    /// </summary>
    private static async Task RunWithoutTablesHasNoStatus()
    {
        // Act
        var section = await Ran(new NotebookScriptResult { Succeeded = true });

        // Assert
        Check(section.Results.Count == 0 && section.ResultStatus.Length == 0);
    }

    /// <summary>
    /// Tests FailedScriptKeepsItsError.
    /// </summary>
    private static async Task FailedScriptKeepsItsError()
    {
        // Act
        var section = await Ran(new NotebookScriptResult { Error = "CS0103" });

        // Assert
        Check(section.State == NotebookRunState.Failed && section.StatusMessage == "CS0103");
    }

    /// <summary>
    /// Tests FailedScriptHasADefaultError.
    /// </summary>
    private static async Task FailedScriptHasADefaultError()
    {
        // Act
        var section = await Ran(new NotebookScriptResult());

        // Assert
        Check(section.StatusMessage == "The script failed.");
    }

    /// <summary>
    /// Tests SuccessfulScriptIsLogged.
    /// </summary>
    private static async Task SuccessfulScriptIsLogged()
    {
        // Act
        var section = await Ran(new NotebookScriptResult { Succeeded = true });

        // Assert
        Check(section.State == NotebookRunState.Succeeded && section.Log.Entries
            .Single().Message == "The script ran well.");
    }

    #endregion
}
