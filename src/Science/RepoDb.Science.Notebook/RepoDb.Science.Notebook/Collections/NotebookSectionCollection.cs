using System.Collections.ObjectModel;
using System.ComponentModel;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// The sections of a notebook, in order. It holds one imports section at most and keeps SQL section names unique.
/// </summary>
public sealed class NotebookSectionCollection : ObservableCollection<NotebookSection>
{
    #region Private Variables

    private readonly Action _changed;
    private int _logLimit = NotebookLog.DefaultLimit;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an empty collection.
    /// </summary>
    /// <param name="messages">The messages of the notebook.</param>
    /// <param name="changed">Called when the sections or their saved state change.</param>
    internal NotebookSectionCollection(NotebookMessages messages,
        Action changed)
    {
        Messages = messages;
        _changed = changed;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets whether there is an imports section.
    /// </summary>
    public bool HasImports => this
        .OfType<NotebookImportsSection>()
        .Any();

    /// <summary>
    /// Gets the using directives of the notebook.
    /// </summary>
    internal string ImportsScript => this
        .OfType<NotebookImportsSection>()
        .FirstOrDefault()?.Script ?? string.Empty;

    /// <summary>
    /// Gets or sets the number of entries each section log keeps.
    /// </summary>
    internal int LogLimit
    {
        get => _logLimit;
        set
        {
            _logLimit = Math.Max(1,
                value);
            foreach (var section in this
                .OfType<NotebookExecutableSection>())
            {
                section.Log.Limit = _logLimit;
            }
        }
    }

    /// <summary>
    /// Gets the messages of the notebook.
    /// </summary>
    internal NotebookMessages Messages { get; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Exchanges the places of two sections.
    /// </summary>
    /// <param name="first">The first section.</param>
    /// <param name="second">The second section.</param>
    /// <returns>Whether two different sections of the collection were exchanged.</returns>
    public bool Swap(NotebookSection first,
        NotebookSection second)
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        var one = IndexOf(first);
        var other = IndexOf(second);
        if (one < 0 || other < 0 || one == other)
        {
            return false;
        }
        Exchange(Math.Min(one,
                other),
            Math.Max(one,
                other));
        return true;
    }

    #endregion

    #region Internal Methods

    /// <summary>
    /// Gives a SQL section a new name when its name is not usable.
    /// </summary>
    /// <param name="section">The section.</param>
    internal void EnsureName(NotebookCodeSection section)
    {
        if (section.Language == NotebookCodeLanguage.Sql
            && !(NotebookSectionNames.IsValid(section.Name) && IsNameAvailable(section.Name,
                section)))
        {
            section.SetName(NotebookSectionNames.Next(NamesExcept(section)));
        }
    }

    /// <summary>
    /// Gets the results of the SQL sections by name.
    /// </summary>
    /// <returns>The results.</returns>
    internal IReadOnlyDictionary<string, IReadOnlyList<NotebookTable>> GetSqlResults() => this
        .OfType<NotebookCodeSection>()
        .Where(section => section.Language == NotebookCodeLanguage.Sql && section.Results.Count > 0)
        .ToDictionary(section => section.Name,
            section => section.Results,
            StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Checks that no other SQL section has a name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="section">The section that wants it.</param>
    /// <returns>Whether the name is free.</returns>
    internal bool IsNameAvailable(string name,
        NotebookCodeSection section) => !NamesExcept(section)
            .Contains(name);

    /// <summary>
    /// Adds saved sections, keeping their valid names and every imports section.
    /// </summary>
    /// <param name="sections">The sections.</param>
    internal void Load(IReadOnlyList<NotebookSection> sections)
    {
        NameLoaded(sections
            .OfType<NotebookCodeSection>()
            .Where(section => section.Language == NotebookCodeLanguage.Sql)
            .ToList());
        foreach (var section in sections)
        {
            Attach(section);
            base.InsertItem(Count,
                section);
        }
        _changed();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Removes every section.
    /// </summary>
    protected override void ClearItems()
    {
        foreach (var section in this)
        {
            Detach(section);
        }
        base.ClearItems();
        _changed();
    }

    /// <summary>
    /// Inserts a section.
    /// </summary>
    /// <param name="index">The position.</param>
    /// <param name="item">The section.</param>
    protected override void InsertItem(int index,
        NotebookSection item)
    {
        Validate(item,
            null);
        Attach(item);
        if (item is NotebookCodeSection code)
        {
            EnsureName(code);
        }
        base.InsertItem(index,
            item);
        _changed();
    }

    /// <summary>
    /// Moves a section.
    /// </summary>
    /// <param name="oldIndex">The current position.</param>
    /// <param name="newIndex">The new position.</param>
    protected override void MoveItem(int oldIndex,
        int newIndex)
    {
        base.MoveItem(oldIndex,
            newIndex);
        _changed();
    }

    /// <summary>
    /// Removes a section.
    /// </summary>
    /// <param name="index">The position.</param>
    protected override void RemoveItem(int index)
    {
        Detach(this[index]);
        base.RemoveItem(index);
        _changed();
    }

    /// <summary>
    /// Replaces a section.
    /// </summary>
    /// <param name="index">The position.</param>
    /// <param name="item">The new section.</param>
    protected override void SetItem(int index,
        NotebookSection item)
    {
        var replaced = this[index];
        Validate(item,
            replaced);
        Detach(replaced);
        Attach(item);
        base.SetItem(index,
            item);
        if (item is NotebookCodeSection code)
        {
            EnsureName(code);
        }
        _changed();
    }

    /// <summary>
    /// Takes ownership of a section.
    /// </summary>
    /// <param name="section">The section.</param>
    private void Attach(NotebookSection section)
    {
        section.Owner = this;
        section.PropertyChanged += OnSectionChanged;
        if (section is NotebookExecutableSection executable)
        {
            executable.Log.Adopt(_logLimit);
        }
        section.OnAttached();
    }

    /// <summary>
    /// Releases a section.
    /// </summary>
    /// <param name="section">The section.</param>
    private void Detach(NotebookSection section)
    {
        section.Owner = null;
        section.PropertyChanged -= OnSectionChanged;
    }

    /// <summary>
    /// Exchanges two positions with moves, so a section is never in two places.
    /// </summary>
    /// <param name="low">The earlier position.</param>
    /// <param name="high">The later position.</param>
    private void Exchange(int low,
        int high)
    {
        Move(high,
            low);
        if (high > low + 1)
        {
            Move(low + 1,
                high);
        }
    }

    /// <summary>
    /// Names the loaded SQL sections that have no usable name, without taking a name a later one has.
    /// </summary>
    /// <param name="sections">The SQL sections.</param>
    private static void NameLoaded(IReadOnlyList<NotebookCodeSection> sections)
    {
        var taken = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var unnamed = sections
            .Where(section => !(NotebookSectionNames.IsValid(section.Name) && taken.Add(section.Name)))
            .ToList();
        foreach (var section in unnamed)
        {
            var name = NotebookSectionNames.Next(taken);
            taken.Add(name);
            section.SetName(name);
        }
    }

    /// <summary>
    /// Gets the names of the other SQL sections.
    /// </summary>
    /// <param name="section">The section to leave out.</param>
    /// <returns>The names.</returns>
    private HashSet<string> NamesExcept(NotebookCodeSection section) => this
        .OfType<NotebookCodeSection>()
        .Where(other => other != section && other.Language == NotebookCodeLanguage.Sql)
        .Select(other => other.Name)
        .ToHashSet(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Passes on a change of the saved state of a section.
    /// </summary>
    /// <param name="sender">The section.</param>
    /// <param name="e">The change.</param>
    private void OnSectionChanged(object? sender,
        PropertyChangedEventArgs e)
    {
        if (string.Equals(e.PropertyName,
            nameof(NotebookSection.HasChanges),
            StringComparison.Ordinal))
        {
            _changed();
        }
    }

    /// <summary>
    /// Checks that a section can be added.
    /// </summary>
    /// <param name="item">The section.</param>
    /// <param name="replaced">The section it replaces, or null.</param>
    private void Validate(NotebookSection item,
        NotebookSection? replaced)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (item.Owner is not null && item != replaced)
        {
            throw new InvalidOperationException("The section already belongs to a notebook.");
        }
        if (item is NotebookImportsSection && this
            .Any(other => other is NotebookImportsSection && other != replaced))
        {
            throw new InvalidOperationException("A notebook has one imports section at most.");
        }
    }

    #endregion
}
