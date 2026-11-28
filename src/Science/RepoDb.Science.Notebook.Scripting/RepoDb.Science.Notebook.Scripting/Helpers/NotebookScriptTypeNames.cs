namespace RepoDb.Science.Helpers;

/// <summary>
/// Writes the names of types the way a script reads them.
/// </summary>
internal static class NotebookScriptTypeNames
{
    #region Public Methods

    /// <summary>
    /// Gets the name of a type: short in the System namespace and full elsewhere.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The name.</returns>
    public static string Of(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (type.IsArray)
        {
            return Of(type.GetElementType()!) + "[]";
        }
        var name = GenericName(type);
        return type.Namespace is null or "System" ? name : type.Namespace + "." + name;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Gets a name followed by its type arguments, when it declares any of its own.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The name.</returns>
    private static string GenericName(Type type)
    {
        var tick = type.Name.IndexOf('`',
            StringComparison.Ordinal);
        if (tick < 0)
        {
            return type.Name;
        }
        var arguments = type.GetGenericArguments()
            .Select(Of);
        return $"{type.Name[..tick]}<{string.Join(", ",
            arguments)}>";
    }

    #endregion
}
