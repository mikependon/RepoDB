namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// A guard with a fixed answer.
/// </summary>
internal sealed class FakeGuard : INotebookExecutionGuard
{
    #region Private Variables

    private readonly string? _reason;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a guard.
    /// </summary>
    /// <param name="reason">Why scripts must not run, or null.</param>
    public FakeGuard(string? reason) => _reason = reason;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the sections checked.
    /// </summary>
    public List<NotebookScriptSection> Checked { get; } = [];

    #endregion

    #region Public Methods

    /// <summary>
    /// Records the section and answers.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The reason, or null.</returns>
    public Task<string?> CheckAsync(NotebookScriptSection section,
        CancellationToken cancellationToken = default)
    {
        Checked.Add(section);
        return Task.FromResult(_reason);
    }

    #endregion
}
