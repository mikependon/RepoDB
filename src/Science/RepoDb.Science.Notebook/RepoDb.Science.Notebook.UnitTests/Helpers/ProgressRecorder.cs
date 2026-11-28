namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// Records progress reports as they are made.
/// </summary>
internal sealed class ProgressRecorder : IProgress<NotebookRunProgress>
{
    #region Properties

    /// <summary>
    /// Gets the reports.
    /// </summary>
    public List<NotebookRunProgress> Reports { get; } = [];

    #endregion

    #region Public Methods

    /// <summary>
    /// Records a report.
    /// </summary>
    /// <param name="value">The report.</param>
    public void Report(NotebookRunProgress value) => Reports.Add(value);

    #endregion
}
