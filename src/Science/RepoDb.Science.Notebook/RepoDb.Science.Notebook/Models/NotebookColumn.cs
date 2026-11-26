namespace RepoDb.Science;

/// <summary>
/// A column of a notebook table.
/// </summary>
public sealed class NotebookColumn
{
    #region Constructors

    /// <summary>
    /// Creates a column.
    /// </summary>
    /// <param name="name">The column name.</param>
    /// <param name="typeName">The value type name.</param>
    public NotebookColumn(string name,
        string typeName = "string")
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(typeName);
        Name = name;
        TypeName = typeName;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the column name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the value type name.
    /// </summary>
    public string TypeName { get; }

    #endregion
}
