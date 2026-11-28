namespace RepoDb.Science.Helpers;

/// <summary>
/// The rules for the names of SQL sections.
/// </summary>
internal static class NotebookSectionNames
{
    #region Public Variables

    /// <summary>
    /// What a generated name starts with.
    /// </summary>
    public const string Prefix = "SQLSection";

    #endregion

    #region Public Methods

    /// <summary>
    /// Checks that a name is written like a variable name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>Whether the name is valid.</returns>
    public static bool IsValid(string? name) => !string.IsNullOrEmpty(name)
        && (char.IsLetter(name[0]) || name[0] == '_')
        && name
            .All(character => char.IsLetterOrDigit(character) || character == '_');

    /// <summary>
    /// Gets the first generated name that is not taken.
    /// </summary>
    /// <param name="taken">The taken names, compared without case.</param>
    /// <returns>The name.</returns>
    public static string Next(IReadOnlySet<string> taken)
    {
        var number = 1;
        while (taken.Contains(Prefix + number))
        {
            number++;
        }
        return Prefix + number;
    }

    #endregion
}
