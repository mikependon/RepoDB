namespace RepoDb.Science;

/// <summary>
/// A SQL script to run.
/// </summary>
public sealed class NotebookSqlRequest
{
    #region Constructors

    /// <summary>
    /// Creates a SQL request.
    /// </summary>
    /// <param name="script">The SQL script.</param>
    /// <param name="connectionId">The connection identifier.</param>
    /// <param name="rowLimit">The rows kept for each table.</param>
    public NotebookSqlRequest(string script,
        Guid? connectionId = null,
        int rowLimit = NotebookExecutionContext.DefaultRowLimit)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(script);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rowLimit);
        Script = script;
        ConnectionId = connectionId;
        RowLimit = rowLimit;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the connection identifier.
    /// </summary>
    public Guid? ConnectionId { get; }

    /// <summary>
    /// Gets the rows kept for each table.
    /// </summary>
    public int RowLimit { get; }

    /// <summary>
    /// Gets the SQL script.
    /// </summary>
    public string Script { get; }

    #endregion
}
