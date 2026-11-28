namespace RepoDb.Science;

/// <summary>
/// The names a C# script can use without declaring them.
/// </summary>
public sealed class NotebookScriptGlobals
{
    #region Constructors

    /// <summary>
    /// Creates the globals of a script.
    /// </summary>
    /// <param name="notebook">What the script sees as Notebook.</param>
    internal NotebookScriptGlobals(NotebookScriptHost notebook)
    {
        ArgumentNullException.ThrowIfNull(notebook);
        Notebook = notebook;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the notebook of the script.
    /// </summary>
    public NotebookScriptHost Notebook { get; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Writes a value to the output, the same as <c>Notebook.Print</c>.
    /// </summary>
    /// <param name="value">The value.</param>
    public void Print(object? value) => Notebook.Print(value);

    #endregion
}
