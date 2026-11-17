namespace RepoDb.Science;

/// <summary>
/// Contains a value and its frequency.
/// </summary>
public sealed class ValueCount
{
    #region Constructors

    /// <summary>
    /// Creates a snapshot.
    /// </summary>
    /// <param name="value">The value, or null for missing data.</param>
    /// <param name="count">The occurrence count.</param>
    public ValueCount(object? value,
        int count)
    {
        Value = value;
        Count = count;
    }

    #endregion

    #region Properties

    /// <summary>
    /// The value, or null for missing data.
    /// </summary>
    public object? Value { get; }

    /// <summary>
    /// The occurrence count.
    /// </summary>
    public int Count { get; }

    #endregion
}
