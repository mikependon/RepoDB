namespace RepoDb.Science;

/// <summary>
/// Describes a column snapshot.
/// </summary>
public sealed class ColumnInfo
{
    #region Constructors

    /// <summary>
    /// Creates a snapshot.
    /// </summary>
    /// <param name="name">The column name.</param>
    /// <param name="dataType">The declared CLR type.</param>
    /// <param name="allowDBNull">Whether nulls are permitted.</param>
    /// <param name="nullCount">The missing value count.</param>
    /// <param name="nonNullCount">The present value count.</param>
    public ColumnInfo(string name,
        Type dataType,
        bool allowDBNull,
        int nullCount,
        int nonNullCount)
    {
        Name = name;
        DataType = dataType;
        AllowDBNull = allowDBNull;
        NullCount = nullCount;
        NonNullCount = nonNullCount;
    }

    #endregion

    #region Properties

    /// <summary>
    /// The column name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The declared CLR type.
    /// </summary>
    public Type DataType { get; }

    /// <summary>
    /// Whether nulls are permitted.
    /// </summary>
    public bool AllowDBNull { get; }

    /// <summary>
    /// The missing value count.
    /// </summary>
    public int NullCount { get; }

    /// <summary>
    /// The present value count.
    /// </summary>
    public int NonNullCount { get; }

    #endregion
}
