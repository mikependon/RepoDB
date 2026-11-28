namespace RepoDb.Science;

/// <summary>
/// The texts of the problems of C# scripts. A host overrides what differs. An empty text removes a log line.
/// </summary>
public class NotebookScriptingMessages
{
    #region Constructors

    /// <summary>
    /// Creates the default texts.
    /// </summary>
    public NotebookScriptingMessages()
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the default texts.
    /// </summary>
    public static NotebookScriptingMessages Default { get; } = new();

    #endregion

    #region Public Methods

    /// <summary>
    /// Written for a line of the code that does not compile.
    /// </summary>
    /// <param name="line">The line of the code.</param>
    /// <param name="id">The compiler identifier.</param>
    /// <param name="message">The compiler message.</param>
    /// <returns>The text.</returns>
    public virtual string CodeError(int line,
        string id,
        string message) => $"Line {line}: {id}: {message}";

    /// <summary>
    /// Written first when a script uses the database while database access is off.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string DatabaseAccessNotAllowed() => "C# scripts cannot access the database directly while database access is turned off.";

    /// <summary>
    /// The error of a data reader that was closed before it was read.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string DataReaderClosed() => "The data reader is closed.";

    /// <summary>
    /// Why a method that takes a database connection may not be used.
    /// </summary>
    /// <param name="methodName">The method.</param>
    /// <returns>The text.</returns>
    public virtual string ForbiddenDatabaseMethod(string methodName) => $"{methodName} works on a database connection";

    /// <summary>
    /// Why a database type may not be used.
    /// </summary>
    /// <param name="typeName">The type.</param>
    /// <returns>The text.</returns>
    public virtual string ForbiddenDatabaseType(string typeName) => $"{typeName} is a database connection or command";

    /// <summary>
    /// Why a reference or load directive may not be used.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string ForbiddenDirective() => "#r and #load load code that cannot be checked";

    /// <summary>
    /// Why dynamic may not be used.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string ForbiddenDynamic() => "dynamic binds at run time, so it cannot be checked";

    /// <summary>
    /// Why a type may not be used.
    /// </summary>
    /// <param name="typeName">The type.</param>
    /// <returns>The text.</returns>
    public virtual string ForbiddenType(string typeName) => $"{typeName} is not allowed";

    /// <summary>
    /// Why a type may not be looked up by its name.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string ForbiddenTypeLookup() => "Type.GetType finds a type by its name";

    /// <summary>
    /// Written for each use of what is not allowed.
    /// </summary>
    /// <param name="reason">Why it is not allowed.</param>
    /// <param name="line">The line of the code.</param>
    /// <returns>The text.</returns>
    public virtual string ForbiddenUse(string reason,
        int line) => $"{reason} (line {line})";

    /// <summary>
    /// Written for a line of the imports that does not compile.
    /// </summary>
    /// <param name="line">The line of the imports.</param>
    /// <param name="id">The compiler identifier.</param>
    /// <param name="message">The compiler message.</param>
    /// <returns>The text.</returns>
    public virtual string ImportsError(int line,
        string id,
        string message) => $"Imports, line {line}: {id}: {message}";

    /// <summary>
    /// Written when the imports hold more than using directives.
    /// </summary>
    /// <param name="problem">The first problem.</param>
    /// <returns>The text.</returns>
    public virtual string InvalidImports(string problem) => $"The imports have problems: {problem}";

    /// <summary>
    /// Written when a script throws.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>The text.</returns>
    public virtual string RuntimeError(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        return $"{exception.GetType().Name}: {exception.Message}";
    }

    /// <summary>
    /// The error of a SQL section that has no result to read.
    /// </summary>
    /// <param name="sectionName">The name of the section.</param>
    /// <returns>The text.</returns>
    public virtual string SqlResultMissing(string sectionName) => $"The SQL section '{sectionName}' has no result. Run it first, and check its name.";

    /// <summary>
    /// The error of a value that was not set.
    /// </summary>
    /// <param name="name">The name of the value.</param>
    /// <returns>The text.</returns>
    public virtual string ValueMissing(string name) => $"The notebook context has no value named '{name}'. Set it in a C# section that runs before this one.";

    /// <summary>
    /// The error of a value that is read as another type.
    /// </summary>
    /// <param name="name">The name of the value.</param>
    /// <param name="actualType">The type of the value, or null when the value is null.</param>
    /// <param name="expectedType">The type it is read as.</param>
    /// <returns>The text.</returns>
    public virtual string ValueTypeMismatch(string name,
        Type? actualType,
        Type expectedType)
    {
        ArgumentNullException.ThrowIfNull(expectedType);
        return $"The value '{name}' is a {actualType?.Name ?? "null"}, not a {expectedType.Name}.";
    }

    #endregion
}
