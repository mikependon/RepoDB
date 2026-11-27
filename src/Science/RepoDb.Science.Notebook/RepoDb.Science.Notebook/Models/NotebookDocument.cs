using System.ComponentModel;
using System.Text.Json;
using RepoDb.Science.Serialization;
using RepoDb.Science.Services;

namespace RepoDb.Science;

/// <summary>
/// A notebook: its sections and the file it is kept in.
/// </summary>
public sealed class NotebookDocument : INotifyPropertyChanged
{
    #region Public Variables

    /// <summary>
    /// The description of the header of a new notebook.
    /// </summary>
    public const string DefaultDescription = "Add a description of this notebook: what it is for and how to use it.";

    /// <summary>
    /// The using directives of a new notebook.
    /// </summary>
    public const string DefaultImports = "using Microsoft.Data.SqlClient;\nusing RepoDb;\nusing RepoDb.Data;\nusing RepoDb.Science;";

    /// <summary>
    /// The extension of a notebook file.
    /// </summary>
    public const string Extension = ".rdbnb";

    /// <summary>
    /// The newest file format this version reads and the one it writes.
    /// </summary>
    public const int FormatVersion = 1;

    #endregion

    #region Private Variables

    private string? _fileName;
    private NotebookSection[] _savedSections = [];

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a notebook without sections that uses the default messages.
    /// </summary>
    public NotebookDocument()
        : this(NotebookMessages.Default)
    {
    }

    /// <summary>
    /// Creates a notebook without sections.
    /// </summary>
    /// <param name="messages">The texts the notebook writes.</param>
    public NotebookDocument(NotebookMessages messages)
    {
        ArgumentNullException.ThrowIfNull(messages);
        Sections = new NotebookSectionCollection(messages,
            () => OnPropertyChanged(nameof(HasChanges)));
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the full path of the file, or null before the first save.
    /// </summary>
    public string? FileName
    {
        get => _fileName;
        private set
        {
            _fileName = value;
            OnPropertyChanged(nameof(FileName));
            OnPropertyChanged(nameof(Name));
        }
    }

    /// <summary>
    /// Gets whether sections or their content differ from what was last saved.
    /// </summary>
    public bool HasChanges => !_savedSections
            .SequenceEqual(Sections)
        || Sections
            .Any(section => section.HasChanges);

    /// <summary>
    /// Gets the data the host keeps in the file, preserved as it is.
    /// </summary>
    public IDictionary<string, JsonElement> HostData { get; } = new Dictionary<string, JsonElement>(StringComparer.Ordinal);

    /// <summary>
    /// Gets or sets the number of entries each section log keeps.
    /// </summary>
    public int LogLimit
    {
        get => Sections.LogLimit;
        set => Sections.LogLimit = value;
    }

    /// <summary>
    /// Gets the texts the notebook writes.
    /// </summary>
    public NotebookMessages Messages => Sections.Messages;

    /// <summary>
    /// Gets the name of the notebook, taken from its file name.
    /// </summary>
    public string Name => Path.GetFileNameWithoutExtension(FileName) ?? string.Empty;

    /// <summary>
    /// Gets the sections, in order.
    /// </summary>
    public NotebookSectionCollection Sections { get; }

    #endregion

    #region Events

    /// <summary>
    /// Occurs when a property changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion

    #region Public Methods

    /// <summary>
    /// Creates a notebook with a header, the imports, a SQL section and a C# section.
    /// </summary>
    /// <param name="title">The title of the header.</param>
    /// <param name="imports">The using directives.</param>
    /// <param name="messages">The texts the notebook writes, or null for the default ones.</param>
    /// <returns>The notebook.</returns>
    public static NotebookDocument Create(string title = "",
        string imports = DefaultImports,
        NotebookMessages? messages = null)
    {
        var document = new NotebookDocument(messages ?? NotebookMessages.Default);
        document.Sections.Add(new NotebookHeaderSection { Title = title, Description = document.Messages.NewNotebookDescription() });
        document.Sections.Add(new NotebookImportsSection { Script = imports });
        document.Sections.Add(new NotebookCodeSection());
        document.Sections.Add(new NotebookCodeSection { Language = NotebookCodeLanguage.CSharp });
        return document;
    }

    /// <summary>
    /// Opens a notebook file with the default messages. Nothing is run.
    /// </summary>
    /// <param name="fileName">The file.</param>
    /// <returns>The notebook.</returns>
    public static NotebookDocument Open(string fileName) => Open(fileName,
        NotebookMessages.Default);

    /// <summary>
    /// Opens a notebook file. Nothing is run.
    /// </summary>
    /// <param name="fileName">The file.</param>
    /// <param name="messages">The texts the notebook writes.</param>
    /// <returns>The notebook.</returns>
    public static NotebookDocument Open(string fileName,
        NotebookMessages messages)
    {
        ArgumentNullException.ThrowIfNull(messages);
        var path = FullPathOf(fileName);
        return Loaded(NotebookFileStore.Read(path),
            path,
            messages);
    }

    /// <summary>
    /// Opens a notebook file with the default messages. Nothing is run.
    /// </summary>
    /// <param name="fileName">The file.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The notebook.</returns>
    public static Task<NotebookDocument> OpenAsync(string fileName,
        CancellationToken cancellationToken = default) => OpenAsync(fileName,
            NotebookMessages.Default,
            cancellationToken);

    /// <summary>
    /// Opens a notebook file. Nothing is run.
    /// </summary>
    /// <param name="fileName">The file.</param>
    /// <param name="messages">The texts the notebook writes.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The notebook.</returns>
    public static async Task<NotebookDocument> OpenAsync(string fileName,
        NotebookMessages messages,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(messages);
        var path = FullPathOf(fileName);
        var model = await NotebookFileStore.ReadAsync(path,
            cancellationToken)
            .ConfigureAwait(true);
        return Loaded(model,
            path,
            messages);
    }

    /// <summary>
    /// Removes what the last runs produced in every section.
    /// </summary>
    public void ClearOutput()
    {
        foreach (var section in Sections
            .OfType<NotebookExecutableSection>())
        {
            section.ClearOutput();
        }
    }

    /// <summary>
    /// Runs the imports and then the other sections in order, stopping at the first failure.
    /// </summary>
    /// <param name="context">What the run needs.</param>
    /// <param name="progress">Receives a report after each section.</param>
    /// <param name="cancellationToken">Stops the run.</param>
    /// <returns>The outcome.</returns>
    public Task<NotebookRunResult> ExecuteAsync(NotebookExecutionContext context,
        IProgress<NotebookRunProgress>? progress = null,
        CancellationToken cancellationToken = default) => NotebookRunner.RunAsync(GetRunPlan(),
            context,
            Messages,
            progress,
            cancellationToken);

    /// <summary>
    /// Gets the sections that a run of the notebook runs, in order and with their names.
    /// </summary>
    /// <returns>The steps.</returns>
    public IReadOnlyList<NotebookRunStep> GetRunPlan() => NotebookRunner.Plan(Sections,
        Messages);

    /// <summary>
    /// Saves the notebook to its file.
    /// </summary>
    public void Save() => Save(CurrentFileName());

    /// <summary>
    /// Saves the notebook to a file, which becomes its file.
    /// </summary>
    /// <param name="fileName">The file.</param>
    public void Save(string fileName)
    {
        var path = FullPathOf(fileName);
        var saved = Capture();
        NotebookFileStore.Write(path,
            NotebookFileWriter.Write(this));
        MarkSaved(path,
            saved);
    }

    /// <summary>
    /// Saves the notebook to its file.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task of the save.</returns>
    public Task SaveAsync(CancellationToken cancellationToken = default) => SaveAsync(CurrentFileName(),
        cancellationToken);

    /// <summary>
    /// Saves the notebook to a file, which becomes its file.
    /// </summary>
    /// <param name="fileName">The file.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task of the save.</returns>
    public async Task SaveAsync(string fileName,
        CancellationToken cancellationToken = default)
    {
        var path = FullPathOf(fileName);
        var saved = Capture();
        await NotebookFileStore.WriteAsync(path,
            NotebookFileWriter.Write(this),
            cancellationToken)
            .ConfigureAwait(true);
        MarkSaved(path,
            saved);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Remembers the sections and their content as they are written.
    /// </summary>
    /// <returns>The sections with their content.</returns>
    private (NotebookSection Section, string Content)[] Capture() => Sections
        .Select(section => (section, section.ComposeContent()))
        .ToArray();

    /// <summary>
    /// Gets the file of the notebook.
    /// </summary>
    /// <returns>The file.</returns>
    private string CurrentFileName() => FileName
        ?? throw new InvalidOperationException("The notebook has no file yet. Save it with a file name.");

    /// <summary>
    /// Gets the full path of a file name.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <returns>The full path.</returns>
    private static string FullPathOf(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        return Path.GetFullPath(fileName);
    }

    /// <summary>
    /// Builds a notebook that has no changes from a file.
    /// </summary>
    /// <param name="model">The content of the file.</param>
    /// <param name="path">The file.</param>
    /// <param name="messages">The texts the notebook writes.</param>
    /// <returns>The notebook.</returns>
    private static NotebookDocument Loaded(NotebookFileModel model,
        string path,
        NotebookMessages messages)
    {
        var document = NotebookFileReader.Read(model,
            messages);
        document.MarkSaved(path,
            document.Capture());
        return document;
    }

    /// <summary>
    /// Records what was saved.
    /// </summary>
    /// <param name="path">The file.</param>
    /// <param name="saved">The sections with the content that was saved.</param>
    private void MarkSaved(string path,
        (NotebookSection Section, string Content)[] saved)
    {
        _savedSections = saved
            .Select(item => item.Section)
            .ToArray();
        foreach (var (section, content) in saved)
        {
            section.MarkSaved(content);
        }
        FileName = path;
        OnPropertyChanged(nameof(HasChanges));
    }

    /// <summary>
    /// Raises the property changed event.
    /// </summary>
    /// <param name="propertyName">The changed property.</param>
    private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this,
        new PropertyChangedEventArgs(propertyName));

    #endregion
}
