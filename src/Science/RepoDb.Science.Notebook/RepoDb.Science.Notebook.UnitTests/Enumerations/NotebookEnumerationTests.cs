namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the enumeration names that files and hosts depend on.
/// </summary>
internal static class NotebookEnumerationTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        SectionTypesAreTheFourKinds();
        CodeLanguagesKeepTheirFileNames();
        LogLevelsKeepTheirFileNames();
        FileSectionKindsKeepTheirFileNames();
        ChartTypesFollowTheNameOrder();
        ChartBucketsFollowTheWidthOrder();
        RunStatesStartPending();
        ImportKindsAreTheThreeForms();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Tests SectionTypesAreTheFourKinds.
    /// </summary>
    private static void SectionTypesAreTheFourKinds()
    {
        // Act
        var names = Enum.GetNames<NotebookSectionType>();

        // Assert
        Check(string.Join(',',
            names) == "Header,Imports,Code,Chart");
    }

    /// <summary>
    /// Tests CodeLanguagesKeepTheirFileNames.
    /// </summary>
    private static void CodeLanguagesKeepTheirFileNames()
    {
        // Act
        var names = Enum.GetNames<NotebookCodeLanguage>();

        // Assert
        Check(string.Join(',',
            names) == "Sql,CSharp");
    }

    /// <summary>
    /// Tests LogLevelsKeepTheirFileNames.
    /// </summary>
    private static void LogLevelsKeepTheirFileNames()
    {
        // Act
        var names = Enum.GetNames<NotebookLogLevel>();

        // Assert
        Check(string.Join(',',
            names) == "Debug,Information,Warning,Error,Critical");
    }

    /// <summary>
    /// Tests FileSectionKindsKeepTheirFileNames.
    /// </summary>
    private static void FileSectionKindsKeepTheirFileNames()
    {
        // Act
        var names = Enum.GetNames<NotebookFileSectionKind>();

        // Assert
        Check(string.Join(',',
            names) == "Sql,CSharp,Header,Imports,Chart");
    }

    /// <summary>
    /// Tests ChartTypesFollowTheNameOrder.
    /// </summary>
    private static void ChartTypesFollowTheNameOrder()
    {
        // Act
        var names = Enum.GetNames<NotebookChartType>();

        // Assert
        Check(string.Join(',',
            names) == "StackedLines,Lines,Bars,StackedBars");
    }

    /// <summary>
    /// Tests ChartBucketsFollowTheWidthOrder.
    /// </summary>
    private static void ChartBucketsFollowTheWidthOrder()
    {
        // Act
        var names = Enum.GetNames<NotebookChartBucket>();

        // Assert
        Check(string.Join(',',
            names) == "OneMinute,FiveMinutes,TenMinutes,OneHour,TwelveHours,OneDay");
    }

    /// <summary>
    /// Tests RunStatesStartPending.
    /// </summary>
    private static void RunStatesStartPending()
    {
        // Act
        var state = default(NotebookRunState);

        // Assert
        Check(state == NotebookRunState.Pending);
        Check(Enum.GetNames<NotebookRunState>().Length == 5);
    }

    /// <summary>
    /// Tests ImportKindsAreTheThreeForms.
    /// </summary>
    private static void ImportKindsAreTheThreeForms()
    {
        // Act
        var names = Enum.GetNames<NotebookImportKind>();

        // Assert
        Check(string.Join(',',
            names) == "Namespace,Static,Alias");
    }

    #endregion
}
