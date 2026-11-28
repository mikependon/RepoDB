namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the section log.
/// </summary>
internal static class NotebookLogTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        NewLogIsEmptyWithTheDefaultLimit();
        AddAppendsTheEntry();
        AddStampsTheCurrentTime();
        AddDropsTheOldestAboveTheLimit();
        AddRaisesEntryAdded();
        LoweringTheLimitTrims();
        LimitIsAtLeastOne();
        ClearRemovesEveryEntry();
        LoadReplacesTheEntries();
        LoadKeepsEntriesAboveTheLimit();
        AdoptKeepsEntriesUntilTheNextAdd();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates a log with numbered entries.
    /// </summary>
    /// <param name="count">The number of entries.</param>
    /// <returns>The log.</returns>
    private static NotebookLog LogWith(int count)
    {
        var log = new NotebookLog();
        for (var number = 1; number <= count; number++)
        {
            log.Add(NotebookLogLevel.Information,
                $"Line {number}");
        }
        return log;
    }

    /// <summary>
    /// Tests NewLogIsEmptyWithTheDefaultLimit.
    /// </summary>
    private static void NewLogIsEmptyWithTheDefaultLimit()
    {
        // Act
        var log = new NotebookLog();

        // Assert
        Check(log.Entries.Count == 0 && log.Limit == NotebookLog.DefaultLimit);
    }

    /// <summary>
    /// Tests AddAppendsTheEntry.
    /// </summary>
    private static void AddAppendsTheEntry()
    {
        // Setup
        var log = new NotebookLog();

        // Act
        log.Add(NotebookLogLevel.Error,
            "Broken");

        // Assert
        Check(log.Entries
            .Single() is { Level: NotebookLogLevel.Error, Message: "Broken" });
    }

    /// <summary>
    /// Tests AddStampsTheCurrentTime.
    /// </summary>
    private static void AddStampsTheCurrentTime()
    {
        // Setup
        var before = DateTimeOffset.Now;

        // Act
        var log = LogWith(1);

        // Assert
        Check(log.Entries[0].Time >= before && log.Entries[0].Time <= DateTimeOffset.Now);
    }

    /// <summary>
    /// Tests AddDropsTheOldestAboveTheLimit.
    /// </summary>
    private static void AddDropsTheOldestAboveTheLimit()
    {
        // Act
        var log = LogWith(NotebookLog.DefaultLimit + 1);

        // Assert
        Check(log.Entries.Count == NotebookLog.DefaultLimit && log.Entries[0].Message == "Line 2");
    }

    /// <summary>
    /// Tests AddRaisesEntryAdded.
    /// </summary>
    private static void AddRaisesEntryAdded()
    {
        // Setup
        var log = new NotebookLog();
        var added = new List<string>();
        log.EntryAdded += (_,
            e) => added.Add(e.Entry.Message);

        // Act
        log.Add(NotebookLogLevel.Debug,
            "Seen");

        // Assert
        Check(added
            .Single() == "Seen");
    }

    /// <summary>
    /// Tests LoweringTheLimitTrims.
    /// </summary>
    private static void LoweringTheLimitTrims()
    {
        // Setup
        var log = LogWith(3);

        // Act
        log.Limit = 2;

        // Assert
        Check(log.Entries.Count == 2 && log.Entries[0].Message == "Line 2");
    }

    /// <summary>
    /// Tests LimitIsAtLeastOne.
    /// </summary>
    private static void LimitIsAtLeastOne()
    {
        // Setup
        var log = LogWith(2);

        // Act
        log.Limit = 0;

        // Assert
        Check(log.Limit == 1 && log.Entries.Count == 1);
    }

    /// <summary>
    /// Tests ClearRemovesEveryEntry.
    /// </summary>
    private static void ClearRemovesEveryEntry()
    {
        // Setup
        var log = LogWith(2);

        // Act
        log.Clear();

        // Assert
        Check(log.Entries.Count == 0);
    }

    /// <summary>
    /// Tests LoadReplacesTheEntries.
    /// </summary>
    private static void LoadReplacesTheEntries()
    {
        // Setup
        var log = LogWith(2);
        var saved = new NotebookLogEntry(NotebookLogLevel.Warning,
            "Saved",
            DateTimeOffset.UnixEpoch);

        // Act
        log.Load([saved]);

        // Assert
        Check(ReferenceEquals(log.Entries
            .Single(),
            saved));
    }

    /// <summary>
    /// Tests LoadKeepsEntriesAboveTheLimit.
    /// </summary>
    private static void LoadKeepsEntriesAboveTheLimit()
    {
        // Setup
        var log = new NotebookLog { Limit = 1 };

        // Act
        log.Load(LogWith(3).Entries);

        // Assert
        Check(log.Entries.Count == 3);
    }

    /// <summary>
    /// Tests AdoptKeepsEntriesUntilTheNextAdd.
    /// </summary>
    private static void AdoptKeepsEntriesUntilTheNextAdd()
    {
        // Setup
        var log = LogWith(3);

        // Act
        log.Adopt(2);
        var kept = log.Entries.Count;
        log.Add(NotebookLogLevel.Information,
            "Line 4");

        // Assert
        Check(kept == 3);
        Check(log.Entries.Count == 2);
    }

    #endregion
}
