namespace RepoDb.Science;

/// <summary>
/// Defines a table's serialized layout.
/// </summary>
public enum DataOrientation
{
    /// <summary>
    /// Stores one object or element per row.
    /// </summary>
    Records,

    /// <summary>
    /// Separates column names, row positions, and data.
    /// </summary>
    Split,

    /// <summary>
    /// Identifies each record by its row position.
    /// </summary>
    Index,

    /// <summary>
    /// Groups values by column and row position.
    /// </summary>
    Columns,

    /// <summary>
    /// Stores positional row values.
    /// </summary>
    Values,

    /// <summary>
    /// Stores a schema followed by records.
    /// </summary>
    Table
}
