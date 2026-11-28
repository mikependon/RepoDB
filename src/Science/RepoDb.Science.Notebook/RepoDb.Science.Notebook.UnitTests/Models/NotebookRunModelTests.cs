namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the models of a notebook run.
/// </summary>
internal static class NotebookRunModelTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        FailedSectionResultKeepsTheReason();
        SkippedSectionResultKeepsTheReason();
        SucceededSectionResultHasNoMessage();
        ItemKeepsItsIdentity();
        ItemKeepsItsOutcome();
        ItemRejectsNullSection();
        ItemRejectsNullName();
        ItemRejectsNullMessage();
        ProgressKeepsItsValues();
        ProgressGivesThePercentage();
        ProgressRejectsZeroTotal();
        ProgressRejectsNullItem();
        ResultKeepsItsValues();
        ResultSucceedsWithoutFailure();
        ResultFailsWithFailure();
        ResultRejectsNullItems();
        ResultRejectsNullFailure();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates a run item.
    /// </summary>
    /// <returns>The item.</returns>
    private static NotebookRunItem Item() => new(new NotebookCodeSection(),
        "One",
        NotebookRunState.Succeeded,
        "ok",
        TimeSpan.FromSeconds(1));

    /// <summary>
    /// Tests FailedSectionResultKeepsTheReason.
    /// </summary>
    private static void FailedSectionResultKeepsTheReason()
    {
        // Act
        var result = NotebookSectionResult.Failed("why");

        // Assert
        Check(result.State == NotebookRunState.Failed && result.Message == "why");
    }

    /// <summary>
    /// Tests SkippedSectionResultKeepsTheReason.
    /// </summary>
    private static void SkippedSectionResultKeepsTheReason()
    {
        // Act
        var result = NotebookSectionResult.Skipped("why");

        // Assert
        Check(result.State == NotebookRunState.Skipped && result.Message == "why");
    }

    /// <summary>
    /// Tests SucceededSectionResultHasNoMessage.
    /// </summary>
    private static void SucceededSectionResultHasNoMessage()
    {
        // Act
        var result = NotebookSectionResult.Succeeded();

        // Assert
        Check(result.State == NotebookRunState.Succeeded && result.Message.Length == 0);
    }

    /// <summary>
    /// Tests ItemKeepsItsIdentity.
    /// </summary>
    private static void ItemKeepsItsIdentity()
    {
        // Setup
        var section = new NotebookCodeSection();

        // Act
        var item = new NotebookRunItem(section,
            "Name",
            NotebookRunState.Failed,
            "m",
            TimeSpan.Zero);

        // Assert
        Check(item.Section == section && item.Name == "Name");
    }

    /// <summary>
    /// Tests ItemKeepsItsOutcome.
    /// </summary>
    private static void ItemKeepsItsOutcome()
    {
        // Act
        var item = Item();

        // Assert
        Check(item.State == NotebookRunState.Succeeded && item.Message == "ok" && item.Elapsed.TotalSeconds == 1);
    }

    /// <summary>
    /// Tests ItemRejectsNullSection.
    /// </summary>
    private static void ItemRejectsNullSection()
    {
        // Act
        Action operation = () => _ = new NotebookRunItem(null!,
            "n",
            NotebookRunState.Pending,
            "",
            TimeSpan.Zero);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests ItemRejectsNullName.
    /// </summary>
    private static void ItemRejectsNullName()
    {
        // Act
        Action operation = () => _ = new NotebookRunItem(new NotebookCodeSection(),
            null!,
            NotebookRunState.Pending,
            "",
            TimeSpan.Zero);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests ItemRejectsNullMessage.
    /// </summary>
    private static void ItemRejectsNullMessage()
    {
        // Act
        Action operation = () => _ = new NotebookRunItem(new NotebookCodeSection(),
            "n",
            NotebookRunState.Pending,
            null!,
            TimeSpan.Zero);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests ProgressKeepsItsValues.
    /// </summary>
    private static void ProgressKeepsItsValues()
    {
        // Setup
        var item = Item();

        // Act
        var progress = new NotebookRunProgress(1,
            4,
            item);

        // Assert
        Check(progress.Completed == 1 && progress.Total == 4 && progress.Item == item);
    }

    /// <summary>
    /// Tests ProgressGivesThePercentage.
    /// </summary>
    private static void ProgressGivesThePercentage()
    {
        // Act
        var progress = new NotebookRunProgress(1,
            4,
            Item());

        // Assert
        Check(progress.Percentage == 25);
    }

    /// <summary>
    /// Tests ProgressRejectsZeroTotal.
    /// </summary>
    private static void ProgressRejectsZeroTotal()
    {
        // Act
        Action operation = () => _ = new NotebookRunProgress(0,
            0,
            Item());

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Tests ProgressRejectsNullItem.
    /// </summary>
    private static void ProgressRejectsNullItem()
    {
        // Act
        Action operation = () => _ = new NotebookRunProgress(0,
            1,
            null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests ResultKeepsItsValues.
    /// </summary>
    private static void ResultKeepsItsValues()
    {
        // Setup
        NotebookRunItem[] items = [Item()];

        // Act
        var result = new NotebookRunResult(items,
            false,
            "why",
            TimeSpan.FromSeconds(2));

        // Assert
        Check(ReferenceEquals(result.Items,
            items) && result.FailureMessage == "why" && result.Elapsed.TotalSeconds == 2);
    }

    /// <summary>
    /// Tests ResultSucceedsWithoutFailure.
    /// </summary>
    private static void ResultSucceedsWithoutFailure()
    {
        // Act
        var result = new NotebookRunResult([],
            true,
            "",
            TimeSpan.Zero);

        // Assert
        Check(result.Succeeded);
    }

    /// <summary>
    /// Tests ResultFailsWithFailure.
    /// </summary>
    private static void ResultFailsWithFailure()
    {
        // Act
        var result = new NotebookRunResult([],
            false,
            "why",
            TimeSpan.Zero);

        // Assert
        Check(!result.Succeeded);
    }

    /// <summary>
    /// Tests ResultRejectsNullItems.
    /// </summary>
    private static void ResultRejectsNullItems()
    {
        // Act
        Action operation = () => _ = new NotebookRunResult(null!,
            true,
            "",
            TimeSpan.Zero);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests ResultRejectsNullFailure.
    /// </summary>
    private static void ResultRejectsNullFailure()
    {
        // Act
        Action operation = () => _ = new NotebookRunResult([],
            true,
            null!,
            TimeSpan.Zero);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    #endregion
}
