namespace RepoDb.Science;

/// <summary>
/// The outcome of a SQL script.
/// </summary>
public sealed class NotebookSqlResult
{
    #region Properties

    /// <summary>
    /// Gets why the script failed, or null when it ran.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Gets the number of affected rows, or null when the script only read.
    /// </summary>
    public int? RowsAffected { get; init; }

    /// <summary>
    /// Gets the result status of the host, or null for the default one.
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    /// Gets whether the script ran.
    /// </summary>
    public bool Succeeded => Error is null;

    /// <summary>
    /// Gets the result tables.
    /// </summary>
    public IReadOnlyList<NotebookTable> Tables { get; init; } = [];

    #endregion
}
