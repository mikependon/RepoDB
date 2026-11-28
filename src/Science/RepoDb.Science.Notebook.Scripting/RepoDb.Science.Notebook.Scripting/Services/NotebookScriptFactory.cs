using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// Creates the Roslyn scripts of notebooks. An editor uses it to compile exactly what runs.
/// </summary>
public sealed class NotebookScriptFactory
{
    #region Private Variables

    private const string Requirement = "C# scripts are compiled and loaded while the application runs.";
    private readonly Lazy<ScriptOptions> _options;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a factory. Scripts can use the assemblies the application runs with.
    /// </summary>
    /// <param name="documentation">Gives the documentation of the assembly of a file, for an editor, or null for none.</param>
    /// <param name="isReferenced">Decides whether scripts can use the assembly of a file, or null for every assembly.</param>
    [RequiresDynamicCode(Requirement)]
    [RequiresUnreferencedCode(Requirement)]
    public NotebookScriptFactory(Func<string, DocumentationProvider?>? documentation = null,
        Func<string, bool>? isReferenced = null)
    {
        _options = new Lazy<ScriptOptions>(() => ScriptOptions.Default
            .WithReferences(NotebookScriptReferences.Load(documentation,
                isReferenced))
            .WithImports(DefaultImports));
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the namespaces every script imports.
    /// </summary>
    public static IReadOnlyList<string> DefaultImports { get; } = ["System", "System.Collections.Generic", "System.Data", "System.IO", "System.Linq", "System.Threading.Tasks"];

    /// <summary>
    /// Gets the references and imports every script is compiled with.
    /// </summary>
    public ScriptOptions Options => _options.Value;

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets the number of lines the imports take before the code.
    /// </summary>
    /// <param name="imports">The using directives.</param>
    /// <returns>The number of lines.</returns>
    public static int ImportLinesOf(string imports) => PrefixOf(imports)
        .Count(character => character == '\n');

    /// <summary>
    /// Gets the text that comes before the code in the source of a script.
    /// </summary>
    /// <param name="imports">The using directives.</param>
    /// <returns>The imports ended by one line break, or empty.</returns>
    public static string PrefixOf(string imports)
    {
        ArgumentNullException.ThrowIfNull(imports);
        return string.IsNullOrWhiteSpace(imports) ? string.Empty : imports.TrimEnd() + "\n";
    }

    /// <summary>
    /// Creates the script of the imports followed by the code. Its globals are <see cref="NotebookScriptGlobals"/>.
    /// </summary>
    /// <param name="imports">The using directives.</param>
    /// <param name="code">The code.</param>
    /// <returns>The script, which is not compiled yet.</returns>
    public Script<object> Create(string imports,
        string code)
    {
        ArgumentNullException.ThrowIfNull(code);
        return CSharpScript.Create(PrefixOf(imports) + code,
            Options,
            typeof(NotebookScriptGlobals));
    }

    #endregion
}
