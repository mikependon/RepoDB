namespace RepoDb.Science;

/// <summary>
/// Defines in-memory join modes.
/// </summary>
public enum JoinType
{
    /// <summary>
    /// Includes matching rows.
    /// </summary>
    Inner,
    /// <summary>
    /// Includes every left row.
    /// </summary>
    Left,
    /// <summary>
    /// Includes every right row.
    /// </summary>
    Right,
    /// <summary>
    /// Includes every row from both tables.
    /// </summary>
    Full
}
