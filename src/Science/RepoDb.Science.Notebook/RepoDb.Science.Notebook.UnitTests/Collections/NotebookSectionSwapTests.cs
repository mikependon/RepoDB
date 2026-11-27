namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests swapping sections, planning a run and the new models.
/// </summary>
internal static class NotebookSectionSwapTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        RunSwapTests();
        RunPlanTests();
        RunModelTests();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs the swapping scenarios.
    /// </summary>
    private static void RunSwapTests()
    {
        SwapsDistantSections();
        SwapsAdjacentSections();
        SwapAcceptsAnyOrder();
        SwapKeepsOwnershipAndNames();
        SwapIsAChange();
        SwappingBackIsNoChange();
        SwapOfOneSectionDoesNothing();
        SwapOfAForeignSectionDoesNothing();
        SwapWithAForeignSecondSectionDoesNothing();
        SwapRejectsNullFirst();
        SwapRejectsNullSecond();
    }

    /// <summary>
    /// Runs the planning scenarios.
    /// </summary>
    private static void RunPlanTests()
    {
        PlanPutsImportsFirstAndSkipsHeaders();
        PlanOfAnEmptyNotebookIsEmpty();
        StepKeepsItsValues();
        StepRejectsNullSection();
        StepRejectsNullName();
    }

    /// <summary>
    /// Runs the model scenarios.
    /// </summary>
    private static void RunModelTests()
    {
        SqlResultSucceedsByDefault();
        SqlResultKeepsItsValues();
        SqlResultFailsWithAnError();
        SqlRequestHasItsOwnLog();
        SqlRequestUsesTheDefaultMessages();
        SqlRequestKeepsItsServices();
        ScriptResultKeepsItsStatus();
        PublicReadOnlyCheckRejectsNull();
        PublicImportCheckRejectsNull();
    }

    /// <summary>
    /// Creates a notebook with numbered headers.
    /// </summary>
    /// <returns>The notebook.</returns>
    private static NotebookDocument Numbered()
    {
        var document = new NotebookDocument();
        foreach (var title in new[] { "1", "2", "3", "4" })
        {
            document.Sections.Add(new NotebookHeaderSection { Title = title });
        }
        return document;
    }

    /// <summary>
    /// Writes the order of the sections.
    /// </summary>
    /// <param name="document">The notebook.</param>
    /// <returns>The titles.</returns>
    private static string OrderOf(NotebookDocument document) => string.Concat(document.Sections
        .Select(section => section.Preview));

    /// <summary>
    /// Swaps two positions of a numbered notebook.
    /// </summary>
    /// <param name="first">The first position.</param>
    /// <param name="second">The second position.</param>
    /// <returns>The order.</returns>
    private static string Swapped(int first,
        int second)
    {
        var document = Numbered();
        document.Sections.Swap(document.Sections[first],
            document.Sections[second]);
        return OrderOf(document);
    }

    /// <summary>
    /// Tests SwapsDistantSections.
    /// </summary>
    private static void SwapsDistantSections() => Check(Swapped(0,
        3) == "4231");

    /// <summary>
    /// Tests SwapsAdjacentSections.
    /// </summary>
    private static void SwapsAdjacentSections() => Check(Swapped(1,
        2) == "1324");

    /// <summary>
    /// Tests SwapAcceptsAnyOrder.
    /// </summary>
    private static void SwapAcceptsAnyOrder() => Check(Swapped(2,
        0) == "3214");

    /// <summary>
    /// Tests SwapKeepsOwnershipAndNames.
    /// </summary>
    private static void SwapKeepsOwnershipAndNames()
    {
        // Setup
        var document = NotebookDocument.Create();
        var code = (NotebookCodeSection)document.Sections[2];

        // Act
        document.Sections.Swap(code,
            document.Sections[0]);

        // Assert
        Check(document.Sections[0] == code && code.Owner == document.Sections && code.Name == "SQLSection1");
    }

    /// <summary>
    /// Tests SwapIsAChange.
    /// </summary>
    private static void SwapIsAChange()
    {
        // Setup
        using var folder = new TestFolder();
        var document = Numbered();
        document.Save(folder.FileOf("a.rdbnb"));

        // Act
        var swapped = document.Sections.Swap(document.Sections[0],
            document.Sections[1]);

        // Assert
        Check(swapped && document.HasChanges);
    }

    /// <summary>
    /// Tests SwappingBackIsNoChange.
    /// </summary>
    private static void SwappingBackIsNoChange()
    {
        // Setup
        using var folder = new TestFolder();
        var document = Numbered();
        document.Save(folder.FileOf("a.rdbnb"));
        var (first, last) = (document.Sections[0], document.Sections[3]);
        document.Sections.Swap(first,
            last);

        // Act
        document.Sections.Swap(first,
            last);

        // Assert
        Check(!document.HasChanges);
    }

    /// <summary>
    /// Tests SwapOfOneSectionDoesNothing.
    /// </summary>
    private static void SwapOfOneSectionDoesNothing()
    {
        // Setup
        var document = Numbered();

        // Act
        var swapped = document.Sections.Swap(document.Sections[0],
            document.Sections[0]);

        // Assert
        Check(!swapped && OrderOf(document) == "1234");
    }

    /// <summary>
    /// Tests SwapOfAForeignSectionDoesNothing.
    /// </summary>
    private static void SwapOfAForeignSectionDoesNothing()
    {
        // Setup
        var document = Numbered();

        // Act
        var swapped = document.Sections.Swap(new NotebookHeaderSection(),
            document.Sections[0]);

        // Assert
        Check(!swapped && OrderOf(document) == "1234");
    }

    /// <summary>
    /// Tests SwapWithAForeignSecondSectionDoesNothing.
    /// </summary>
    private static void SwapWithAForeignSecondSectionDoesNothing()
    {
        // Setup
        var document = Numbered();

        // Act
        var swapped = document.Sections.Swap(document.Sections[0],
            new NotebookHeaderSection());

        // Assert
        Check(!swapped);
    }

    /// <summary>
    /// Tests SwapRejectsNullFirst.
    /// </summary>
    private static void SwapRejectsNullFirst()
    {
        // Setup
        var document = Numbered();

        // Act
        Action operation = () => document.Sections.Swap(null!,
            document.Sections[0]);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests SwapRejectsNullSecond.
    /// </summary>
    private static void SwapRejectsNullSecond()
    {
        // Setup
        var document = Numbered();

        // Act
        Action operation = () => document.Sections.Swap(document.Sections[0],
            null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests PlanPutsImportsFirstAndSkipsHeaders.
    /// </summary>
    private static void PlanPutsImportsFirstAndSkipsHeaders()
    {
        // Setup
        var document = NotebookDocument.Create();
        document.Sections.Move(1,
            3);

        // Act
        var plan = document.GetRunPlan();

        // Assert
        Check(plan.Count == 3 && plan[0].Section is NotebookImportsSection);
        Check(string.Join('|',
            plan
                .Select(step => step.Name)) == "Imports|SQLSection1|C# 3");
    }

    /// <summary>
    /// Tests PlanOfAnEmptyNotebookIsEmpty.
    /// </summary>
    private static void PlanOfAnEmptyNotebookIsEmpty() => Check(new NotebookDocument().GetRunPlan().Count == 0);

    /// <summary>
    /// Tests StepKeepsItsValues.
    /// </summary>
    private static void StepKeepsItsValues()
    {
        // Setup
        var section = new NotebookCodeSection();

        // Act
        var step = new NotebookRunStep(section,
            "n");

        // Assert
        Check(step.Section == section && step.Name == "n");
    }

    /// <summary>
    /// Tests StepRejectsNullSection.
    /// </summary>
    private static void StepRejectsNullSection()
    {
        // Act
        Action operation = () => _ = new NotebookRunStep(null!,
            "n");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests StepRejectsNullName.
    /// </summary>
    private static void StepRejectsNullName()
    {
        // Act
        Action operation = () => _ = new NotebookRunStep(new NotebookCodeSection(),
            null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests SqlResultSucceedsByDefault.
    /// </summary>
    private static void SqlResultSucceedsByDefault()
    {
        // Act
        var result = new NotebookSqlResult();

        // Assert
        Check(result.Succeeded && result.Error is null && result.Tables.Count == 0);
        Check(result.RowsAffected is null && result.Status is null);
    }

    /// <summary>
    /// Tests SqlResultKeepsItsValues.
    /// </summary>
    private static void SqlResultKeepsItsValues()
    {
        // Setup
        var table = SampleTables.Timed();

        // Act
        var result = new NotebookSqlResult { RowsAffected = 2, Status = "s", Tables = [table] };

        // Assert
        Check(result is { RowsAffected: 2, Status: "s" } && result.Tables[0] == table);
    }

    /// <summary>
    /// Tests SqlResultFailsWithAnError.
    /// </summary>
    private static void SqlResultFailsWithAnError()
    {
        // Act
        var result = new NotebookSqlResult { Error = "bad" };

        // Assert
        Check(!result.Succeeded && result.Error == "bad");
    }

    /// <summary>
    /// Tests SqlRequestHasItsOwnLog.
    /// </summary>
    private static void SqlRequestHasItsOwnLog()
    {
        // Act
        var request = new NotebookSqlRequest("q");

        // Assert
        Check(request.Log.Entries.Count == 0);
    }

    /// <summary>
    /// Tests SqlRequestUsesTheDefaultMessages.
    /// </summary>
    private static void SqlRequestUsesTheDefaultMessages() => Check(ReferenceEquals(new NotebookSqlRequest("q").Messages,
        NotebookMessages.Default));

    /// <summary>
    /// Tests SqlRequestKeepsItsServices.
    /// </summary>
    private static void SqlRequestKeepsItsServices()
    {
        // Setup
        var log = new NotebookLog();
        var messages = new TagMessages();

        // Act
        var request = new NotebookSqlRequest("q") { Log = log, Messages = messages };

        // Assert
        Check(request.Log == log && request.Messages == messages);
    }

    /// <summary>
    /// Tests ScriptResultKeepsItsStatus.
    /// </summary>
    private static void ScriptResultKeepsItsStatus()
    {
        // Assert
        Check(new NotebookScriptResult().Status is null && new NotebookScriptResult { Status = "s" }.Status == "s");
    }

    /// <summary>
    /// Tests PublicReadOnlyCheckRejectsNull.
    /// </summary>
    private static void PublicReadOnlyCheckRejectsNull()
    {
        // Act
        Action operation = () => NotebookSqlReadOnlyCheck.IsReadOnly(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests PublicImportCheckRejectsNull.
    /// </summary>
    private static void PublicImportCheckRejectsNull()
    {
        // Act
        Action operation = () => NotebookImportValidator.Check(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    #endregion
}
