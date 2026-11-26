namespace RepoDb.Science.Helpers;

/// <summary>
/// Decides, on the safe side, whether a SQL script only reads.
/// </summary>
internal static class NotebookSqlReadOnlyCheck
{
    #region Private Variables

    private static readonly HashSet<string> Forbidden = new(StringComparer.Ordinal)
    {
        "ALTER", "ATTACH", "BACKUP", "BEGIN", "BULK", "CALL", "CHECKPOINT", "CLUSTER", "COMMIT", "COPY", "CREATE", "DBCC",
        "DEALLOCATE", "DECLARE", "DELETE", "DENY", "DETACH", "DISABLE", "DO", "DROP", "DUMP", "ENABLE", "EXEC", "EXECUTE",
        "GRANT", "HANDLER", "INSERT", "INTO", "KILL", "LOAD", "LOCK", "MERGE", "MOVE", "OPENDATASOURCE", "OPENQUERY",
        "OPENROWSET", "OPTIMIZE", "PRAGMA", "PREPARE", "RECEIVE", "RECONFIGURE", "REFRESH", "REINDEX", "RENAME", "RESTORE",
        "REVERT", "REVOKE", "ROLLBACK", "SEND", "SET", "SETUSER", "SHUTDOWN", "TRUNCATE", "UNLOCK", "UPDATE", "UPDATETEXT",
        "UPSERT", "USE", "VACUUM", "WAITFOR", "WRITETEXT"
    };

    #endregion

    #region Public Methods

    /// <summary>
    /// Checks a script.
    /// </summary>
    /// <param name="sql">The script.</param>
    /// <returns>Whether every statement is a plain SELECT or WITH query.</returns>
    public static bool IsReadOnly(string sql)
    {
        var statements = NotebookSqlScanner.Scan(sql);
        return statements is { Count: > 0 } && statements
            .All(IsQuery);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Checks a statement.
    /// </summary>
    /// <param name="statement">The tokens.</param>
    /// <returns>Whether it is a query.</returns>
    private static bool IsQuery(IReadOnlyList<string> statement)
    {
        var first = statement
            .SkipWhile(token => string.Equals(token,
                "(",
                StringComparison.Ordinal))
            .FirstOrDefault();
        return first is "SELECT" or "WITH" && !statement
            .Any(Forbidden.Contains);
    }

    #endregion
}
