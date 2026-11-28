namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// An object with readable, write-only and indexed properties.
/// </summary>
internal sealed class RowObject
{
    #region Private Variables

    private string _secret = string.Empty;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Sets a value that cannot be read.
    /// </summary>
    public string Secret
    {
        set => _secret = value;
    }

    /// <summary>
    /// Gets a character of the secret.
    /// </summary>
    /// <param name="index">The position.</param>
    public char this[int index] => _secret[index];

    #endregion
}
