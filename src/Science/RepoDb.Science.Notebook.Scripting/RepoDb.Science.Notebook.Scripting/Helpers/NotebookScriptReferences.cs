using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace RepoDb.Science.Helpers;

/// <summary>
/// The assemblies scripts are compiled with.
/// </summary>
internal static class NotebookScriptReferences
{
    #region Public Methods

    /// <summary>
    /// References the assemblies the runtime lists for the application.
    /// </summary>
    /// <param name="documentation">Gives the documentation of the assembly of a file, or null for none.</param>
    /// <param name="isReferenced">Decides whether the assembly of a file is referenced, or null for every one.</param>
    /// <returns>The references.</returns>
    [RequiresUnreferencedCode("Scripts are compiled against the assembly files of the application.")]
    public static IReadOnlyList<MetadataReference> Load(Func<string, DocumentationProvider?>? documentation,
        Func<string, bool>? isReferenced) => PathsOf(AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string)
        .Where(isReferenced ?? (static _ => true))
        .Select(path => MetadataReference.CreateFromFile(path,
            documentation: documentation?.Invoke(path)))
        .ToList();

    /// <summary>
    /// Gets the files of a path list that exist.
    /// </summary>
    /// <param name="list">The paths, separated as the platform separates them, or null.</param>
    /// <returns>The paths.</returns>
    public static IEnumerable<string> PathsOf(string? list) => (list ?? string.Empty)
        .Split(Path.PathSeparator,
            StringSplitOptions.RemoveEmptyEntries)
        .Where(File.Exists);

    #endregion
}
