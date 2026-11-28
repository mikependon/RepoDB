using Microsoft.CodeAnalysis;

namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// Compiles the scripts of the tests without running them.
/// </summary>
internal static class Compilations
{
    #region Properties

    /// <summary>
    /// Gets a factory the tests share.
    /// </summary>
    public static NotebookScriptFactory Factory { get; } = new();

    #endregion

    #region Public Methods

    /// <summary>
    /// Compiles a script.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <param name="imports">The imports.</param>
    /// <returns>The compilation.</returns>
    public static Compilation Of(string code,
        string imports = "") => Factory.Create(imports,
            code)
        .GetCompilation();

    #endregion
}
