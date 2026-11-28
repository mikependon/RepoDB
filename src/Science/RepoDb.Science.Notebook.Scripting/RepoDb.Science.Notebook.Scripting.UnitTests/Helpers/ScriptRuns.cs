namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// Runs the scripts of the tests.
/// </summary>
internal static class ScriptRuns
{
    #region Public Variables

    /// <summary>
    /// A script that creates a table with two rows.
    /// </summary>
    public const string TwoRowTable = """var table = new DataTable("T"); table.Columns.Add("Value", typeof(int)); table.Rows.Add(1); table.Rows.Add(2); """;

    #endregion

    #region Public Methods

    /// <summary>
    /// Runs a script without SQL results.
    /// </summary>
    /// <param name="executor">The executor.</param>
    /// <param name="code">The script.</param>
    /// <param name="imports">The imports.</param>
    /// <param name="rowLimit">The row limit.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The outcome.</returns>
    public static Task<NotebookScriptResult> Run(NotebookCSharpExecutor executor,
        string code,
        string imports = "",
        int rowLimit = 10,
        CancellationToken cancellationToken = default) => executor.ExecuteAsync(new NotebookScriptRequest(code,
            imports,
            new Dictionary<string, IReadOnlyList<NotebookTable>>(StringComparer.OrdinalIgnoreCase),
            rowLimit),
        cancellationToken);

    #endregion
}
