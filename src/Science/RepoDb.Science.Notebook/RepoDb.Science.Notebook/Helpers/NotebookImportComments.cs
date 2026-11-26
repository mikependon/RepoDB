using System.Text;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Removes C# comments from lines of imports.
/// </summary>
internal static class NotebookImportComments
{
    #region Public Methods

    /// <summary>
    /// Gets a line without its comments.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <param name="inBlock">Whether a block comment is open.</param>
    /// <returns>The code of the line.</returns>
    public static string Strip(string line,
        ref bool inBlock)
    {
        var code = new StringBuilder();
        for (var index = 0; index < line.Length; index++)
        {
            if (inBlock)
            {
                inBlock = !EndsBlock(line,
                    ref index);
            }
            else if (StartsAt(line,
                index,
                "//"))
            {
                break;
            }
            else if (StartsAt(line,
                index,
                "/*"))
            {
                inBlock = true;
                index++;
            }
            else
            {
                code.Append(line[index]);
            }
        }
        return code.ToString();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Moves to the end of a block comment.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <param name="index">The current position.</param>
    /// <returns>Whether the comment ends in this line.</returns>
    private static bool EndsBlock(string line,
        ref int index)
    {
        var end = line.IndexOf("*/",
            index,
            StringComparison.Ordinal);
        index = end < 0 ? line.Length : end + 1;
        return end >= 0;
    }

    /// <summary>
    /// Checks whether a text is at a position.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <param name="index">The position.</param>
    /// <param name="text">The text.</param>
    /// <returns>Whether it is there.</returns>
    private static bool StartsAt(string line,
        int index,
        string text) => line
        .AsSpan(index)
        .StartsWith(text,
            StringComparison.Ordinal);

    #endregion
}
