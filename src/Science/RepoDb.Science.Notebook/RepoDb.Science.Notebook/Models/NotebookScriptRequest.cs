namespace RepoDb.Science;

/// <summary>
/// A C# script to run.
/// </summary>
public sealed class NotebookScriptRequest
{
    #region Constructors

    /// <summary>
    /// Creates a script request.
    /// </summary>
    /// <param name="code">The C# code.</param>
    /// <param name="imports">The using directives.</param>
    /// <param name="sqlResults">The SQL results by section name.</param>
    /// <param name="rowLimit">The rows kept for each table.</param>
    public NotebookScriptRequest(string code,
        string imports,
        IReadOnlyDictionary<string, IReadOnlyList<NotebookTable>> sqlResults,
        int rowLimit = NotebookExecutionContext.DefaultRowLimit)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(imports);
        ArgumentNullException.ThrowIfNull(sqlResults);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rowLimit);
        Code = code;
        Imports = imports;
        SqlResults = sqlResults;
        RowLimit = rowLimit;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the C# code.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the using directives.
    /// </summary>
    public string Imports { get; }

    /// <summary>
    /// Gets the rows kept for each table.
    /// </summary>
    public int RowLimit { get; }

    /// <summary>
    /// Gets the SQL results by section name.
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyList<NotebookTable>> SqlResults { get; }

    #endregion
}
