namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// Texts of a host that differ from the default ones.
/// </summary>
internal sealed class MarkedMessages : NotebookScriptingMessages
{
    #region Public Methods

    /// <summary>
    /// Writes a compile error.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="message">The message.</param>
    /// <returns>The text.</returns>
    public override string CodeError(int line,
        string id,
        string message) => $"code {line} {id}";

    /// <summary>
    /// Writes the refusal of database access.
    /// </summary>
    /// <returns>The text.</returns>
    public override string DatabaseAccessNotAllowed() => "no database";

    /// <summary>
    /// Writes the error of a closed reader.
    /// </summary>
    /// <returns>The text.</returns>
    public override string DataReaderClosed() => "closed";

    /// <summary>
    /// Writes the reason of a blocked type.
    /// </summary>
    /// <param name="typeName">The type.</param>
    /// <returns>The text.</returns>
    public override string ForbiddenType(string typeName) => $"blocked {typeName}";

    /// <summary>
    /// Writes the refusal of the imports.
    /// </summary>
    /// <param name="problem">The problem.</param>
    /// <returns>The text.</returns>
    public override string InvalidImports(string problem) => "bad imports";

    /// <summary>
    /// Writes a runtime error.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>The text.</returns>
    public override string RuntimeError(Exception exception) => $"threw {exception.Message}";

    /// <summary>
    /// Writes the error of a missing SQL result.
    /// </summary>
    /// <param name="sectionName">The section.</param>
    /// <returns>The text.</returns>
    public override string SqlResultMissing(string sectionName) => $"no result {sectionName}";

    /// <summary>
    /// Writes the error of a missing value.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The text.</returns>
    public override string ValueMissing(string name) => $"no value {name}";

    /// <summary>
    /// Writes the error of a wrong type.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="actualType">The type of the value.</param>
    /// <param name="expectedType">The wanted type.</param>
    /// <returns>The text.</returns>
    public override string ValueTypeMismatch(string name,
        Type? actualType,
        Type expectedType) => $"wrong type {name}";

    #endregion
}
