using System.Text;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Works with the texts of sections.
/// </summary>
internal static class NotebookText
{
    #region Public Methods

    /// <summary>
    /// Joins values so that different lists never give the same text.
    /// </summary>
    /// <param name="parts">The values.</param>
    /// <returns>The joined text.</returns>
    public static string Compose(IEnumerable<string?> parts)
    {
        var text = new StringBuilder();
        foreach (var part in parts)
        {
            text
                .Append(part?.Length ?? -1)
                .Append(':')
                .Append(part);
        }
        return text.ToString();
    }

    /// <summary>
    /// Gets the first line that is not blank.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>The line, or empty.</returns>
    public static string FirstLine(string text) => text
        .Split(['\r', '\n'],
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .FirstOrDefault() ?? string.Empty;

    #endregion
}
