using Microsoft.CodeAnalysis;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the assemblies scripts are compiled with.
/// </summary>
internal static class NotebookScriptReferencesTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        PathsOfNothingIsEmpty();
        PathsOfKeepsTheFilesThatExist();
        LoadReferencesTheAssembliesOfTheApplication();
        LoadLeavesOutWhatTheHostRefuses();
        LoadAsksForTheDocumentationOfEveryAssembly();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Tells whether the notebook assembly is referenced.
    /// </summary>
    /// <param name="references">The references.</param>
    /// <returns>Whether it is.</returns>
    private static bool HasNotebook(IEnumerable<MetadataReference> references) => references
        .Any(reference => reference.Display!.EndsWith("RepoDb.Science.Notebook.dll",
            StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Tests PathsOfNothingIsEmpty.
    /// </summary>
    private static void PathsOfNothingIsEmpty()
    {
        // Act
        var paths = NotebookScriptReferences.PathsOf(null);

        // Assert
        Check(!paths
            .Any());
    }

    /// <summary>
    /// Tests PathsOfKeepsTheFilesThatExist.
    /// </summary>
    private static void PathsOfKeepsTheFilesThatExist()
    {
        // Setup
        var existing = typeof(NotebookScriptReferencesTests).Assembly.Location;
        var list = string.Join(Path.PathSeparator,
            existing,
            string.Empty,
            existing + ".missing");

        // Act
        var paths = NotebookScriptReferences.PathsOf(list);

        // Assert
        Check(paths
            .SequenceEqual([existing]));
    }

    /// <summary>
    /// Tests LoadReferencesTheAssembliesOfTheApplication.
    /// </summary>
    private static void LoadReferencesTheAssembliesOfTheApplication()
    {
        // Act
        var references = NotebookScriptReferences.Load(null,
            null);

        // Assert
        Check(HasNotebook(references));
    }

    /// <summary>
    /// Tests LoadLeavesOutWhatTheHostRefuses.
    /// </summary>
    private static void LoadLeavesOutWhatTheHostRefuses()
    {
        // Act
        var references = NotebookScriptReferences.Load(null,
            path => !path.EndsWith("RepoDb.Science.Notebook.dll",
                StringComparison.OrdinalIgnoreCase));

        // Assert
        Check(references.Count > 0 && !HasNotebook(references));
    }

    /// <summary>
    /// Tests LoadAsksForTheDocumentationOfEveryAssembly.
    /// </summary>
    private static void LoadAsksForTheDocumentationOfEveryAssembly()
    {
        // Setup
        var asked = new List<string>();

        // Act
        var references = NotebookScriptReferences.Load(path =>
            {
                asked.Add(path);
                return null;
            },
            null);

        // Assert
        Check(asked.Count == references.Count);
    }

    #endregion
}
