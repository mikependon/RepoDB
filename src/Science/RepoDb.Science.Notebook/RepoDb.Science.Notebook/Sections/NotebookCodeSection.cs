using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// A SQL or a C# script.
/// </summary>
public sealed class NotebookCodeSection : NotebookScriptSection
{
    #region Private Variables

    private NotebookCodeLanguage _language;
    private string _name = string.Empty;
    private string _script = string.Empty;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an empty SQL section.
    /// </summary>
    public NotebookCodeSection()
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the language of the script.
    /// </summary>
    public NotebookCodeLanguage Language
    {
        get => _language;
        set
        {
            if (SetContent(ref _language,
                NotebookValues.Defined(value,
                    nameof(value))))
            {
                Owner?.EnsureName(this);
            }
        }
    }

    /// <summary>
    /// Gets the name C# scripts use to read the results of a SQL section.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Gets or sets the script.
    /// </summary>
    public string Script
    {
        get => _script;
        set => SetContent(ref _script,
            value ?? string.Empty);
    }

    /// <summary>
    /// Gets the language of the script that runs.
    /// </summary>
    public override NotebookCodeLanguage ScriptLanguage => Language;

    /// <summary>
    /// Gets the script that runs.
    /// </summary>
    public override string ScriptToRun => Script;

    /// <summary>
    /// Gets the kind of section.
    /// </summary>
    public override NotebookSectionType Type => NotebookSectionType.Code;

    #endregion

    #region Public Methods

    /// <summary>
    /// Renames the section when the name is valid and no other SQL section has it.
    /// </summary>
    /// <param name="name">The new name.</param>
    /// <returns>Whether the name was accepted.</returns>
    public bool TryRename(string name)
    {
        var accepted = NotebookSectionNames.IsValid(name)
            && (Language != NotebookCodeLanguage.Sql || Owner?.IsNameAvailable(name,
                this) != false);
        if (accepted)
        {
            SetName(name);
        }
        return accepted;
    }

    #endregion

    #region Internal Methods

    /// <summary>
    /// Sets the name without checking it.
    /// </summary>
    /// <param name="name">The name.</param>
    internal void SetName(string name) => SetContent(ref _name,
        name,
        nameof(Name));

    #endregion

    #region Private Methods

    /// <summary>
    /// Gets the authored values.
    /// </summary>
    /// <returns>The values.</returns>
    private protected override IEnumerable<string?> GetContent() => [Language.ToString(), Name, Script, ConnectionId?.ToString()];

    #endregion
}
