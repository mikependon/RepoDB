using System.ComponentModel;
using System.Runtime.CompilerServices;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// The base of every notebook section.
/// </summary>
public abstract class NotebookSection : INotifyPropertyChanged
{
    #region Private Variables

    private bool _isCollapsed;
    private string? _savedContent;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a section.
    /// </summary>
    private protected NotebookSection()
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets whether the content differs from what was last saved.
    /// </summary>
    public bool HasChanges => _savedContent is not null && !string.Equals(_savedContent,
        ComposeContent(),
        StringComparison.Ordinal);

    /// <summary>
    /// Gets or sets whether the section is closed.
    /// </summary>
    public bool IsCollapsed
    {
        get => _isCollapsed;
        set => SetProperty(ref _isCollapsed,
            value);
    }

    /// <summary>
    /// Gets the first line of the content.
    /// </summary>
    public string Preview => NotebookText.FirstLine(PreviewSource);

    /// <summary>
    /// Gets the kind of section.
    /// </summary>
    public abstract NotebookSectionType Type { get; }

    /// <summary>
    /// Gets or sets the collection that holds the section.
    /// </summary>
    internal NotebookSectionCollection? Owner { get; set; }

    /// <summary>
    /// Gets the text the preview comes from.
    /// </summary>
    private protected abstract string PreviewSource { get; }

    #endregion

    #region Events

    /// <summary>
    /// Occurs when a property changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion

    #region Public Methods

    /// <summary>
    /// Accepts the current content as saved.
    /// </summary>
    public void MarkSaved() => MarkSaved(ComposeContent());

    #endregion

    #region Internal Methods

    /// <summary>
    /// Writes the authored content as one text.
    /// </summary>
    /// <returns>The content.</returns>
    internal string ComposeContent() => NotebookText.Compose(GetContent());

    /// <summary>
    /// Accepts a content as saved.
    /// </summary>
    /// <param name="content">The saved content.</param>
    internal void MarkSaved(string content)
    {
        _savedContent = content;
        OnPropertyChanged(nameof(HasChanges));
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Remembers the content before its first change.
    /// </summary>
    private protected void CaptureContent() => _savedContent ??= ComposeContent();

    /// <summary>
    /// Gets the authored values.
    /// </summary>
    /// <returns>The values.</returns>
    private protected abstract IEnumerable<string?> GetContent();

    /// <summary>
    /// Announces a change of content.
    /// </summary>
    /// <param name="propertyName">The changed property.</param>
    private protected void NotifyContent(string? propertyName)
    {
        OnPropertyChanged(propertyName);
        OnPropertyChanged(nameof(Preview));
        OnPropertyChanged(nameof(HasChanges));
    }

    /// <summary>
    /// Raises the property changed event.
    /// </summary>
    /// <param name="propertyName">The changed property.</param>
    private protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this,
        new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// Changes a value that is part of the content.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="field">The backing field.</param>
    /// <param name="value">The new value.</param>
    /// <param name="propertyName">The changed property.</param>
    /// <returns>Whether the value changed.</returns>
    private protected bool SetContent<T>(ref T field,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field,
            value))
        {
            return false;
        }
        CaptureContent();
        field = value;
        NotifyContent(propertyName);
        return true;
    }

    /// <summary>
    /// Changes a value that is not part of the content.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="field">The backing field.</param>
    /// <param name="value">The new value.</param>
    /// <param name="propertyName">The changed property.</param>
    /// <returns>Whether the value changed.</returns>
    private protected bool SetProperty<T>(ref T field,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field,
            value))
        {
            return false;
        }
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #endregion
}
