using Microsoft.CodeAnalysis;

namespace RepoDb.Science.Helpers;

/// <summary>
/// What a script may not use while it has no database access: the database providers, anything that is a connection,
/// a command or a transaction, and the ways to reach them by name.
/// </summary>
internal sealed class NotebookScriptRestrictions
{
    #region Private Variables

    private static readonly string[] BlockedNamespaces =
    [
        "Microsoft.CodeAnalysis",
        "Microsoft.Data.Sqlite",
        "Microsoft.Data.SqlClient",
        "MySql",
        "MySqlConnector",
        "Npgsql",
        "Oracle",
        "System.Data.SQLite",
        "System.Data.SqlClient",
        "System.Reflection",
        "System.Runtime.InteropServices",
        "System.Runtime.Loader"
    ];
    private static readonly string[] BlockedTypes =
    [
        "RepoDb.Science.NotebookCSharpExecutor",
        "RepoDb.Science.NotebookScriptFactory",
        "System.Activator",
        "System.AppDomain",
        "System.Diagnostics.Process"
    ];
    private static readonly string[] DatabaseInterfaces =
    [
        "System.Data.IDataAdapter",
        "System.Data.IDbCommand",
        "System.Data.IDbConnection",
        "System.Data.IDbTransaction"
    ];
    private readonly NotebookScriptingMessages _messages;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates the restrictions.
    /// </summary>
    /// <param name="messages">The texts of the reasons.</param>
    public NotebookScriptRestrictions(NotebookScriptingMessages messages)
    {
        ArgumentNullException.ThrowIfNull(messages);
        _messages = messages;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets why a symbol may not be used.
    /// </summary>
    /// <param name="symbol">The symbol a name of the script stands for, or null.</param>
    /// <returns>The reason, or null when it may.</returns>
    public string? ReasonOf(ISymbol? symbol) => symbol switch
    {
        ITypeSymbol type => ReasonOfType(type),
        IMethodSymbol method => ReasonOfMethod(method),
        IPropertySymbol property => ReasonOfType(property.Type),
        IFieldSymbol field => ReasonOfType(field.Type),
        _ => null
    };

    #endregion

    #region Private Methods

    /// <summary>
    /// Tells whether a type is, or implements, what holds a database.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>Whether it does.</returns>
    private static bool IsDatabaseType(ITypeSymbol type) => IsDatabaseInterface(type)
        || type.AllInterfaces
            .Any(IsDatabaseInterface);

    /// <summary>
    /// Tells whether a type is one of the database interfaces.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>Whether it is.</returns>
    private static bool IsDatabaseInterface(ITypeSymbol type) => DatabaseInterfaces.Contains(NameOf(type),
        StringComparer.Ordinal);

    /// <summary>
    /// Tells whether a type is blocked by its namespace or its name.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>Whether it is.</returns>
    private static bool IsBlocked(ITypeSymbol type)
    {
        var space = type.ContainingNamespace?.ToDisplayString() ?? string.Empty;
        return BlockedTypes.Contains(NameOf(type),
                StringComparer.Ordinal)
            || BlockedNamespaces
                .Any(blocked => string.Equals(space,
                    blocked,
                    StringComparison.Ordinal) || space.StartsWith(blocked + ".",
                    StringComparison.Ordinal));
    }

    /// <summary>
    /// Gets the name of a type without its nullable mark, which a declared member can carry.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The name.</returns>
    private static string NameOf(ITypeSymbol type) => type.OriginalDefinition
        .ToDisplayString();

    /// <summary>
    /// Gets why a method may not be used: its types may not, it takes a database, or it finds a type by its name.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <returns>The reason, or null.</returns>
    private string? ReasonOfMethod(IMethodSymbol method)
    {
        if ((ReasonOfType(method.ContainingType) ?? ReasonOfType(method.ReturnType)) is { } reason)
        {
            return reason;
        }
        if (method.Parameters
            .Any(parameter => IsDatabaseType(parameter.Type)))
        {
            return _messages.ForbiddenDatabaseMethod(method.Name);
        }
        return IsTypeLookup(method) ? _messages.ForbiddenTypeLookup() : null;
    }

    /// <summary>
    /// Tells whether a method is Type.GetType with a name.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <returns>Whether it is.</returns>
    private static bool IsTypeLookup(IMethodSymbol method) => string.Equals(method.Name,
            "GetType",
            StringComparison.Ordinal)
        && method.Parameters.Length > 0
        && string.Equals(method.ContainingType.ToDisplayString(),
            "System.Type",
            StringComparison.Ordinal);

    /// <summary>
    /// Gets why a type may not be used.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The reason, or null.</returns>
    private string? ReasonOfType(ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol array)
        {
            return ReasonOfType(array.ElementType);
        }
        if (type.TypeKind == TypeKind.Dynamic)
        {
            return _messages.ForbiddenDynamic();
        }
        if (IsBlocked(type))
        {
            return _messages.ForbiddenType(type.ToDisplayString());
        }
        return IsDatabaseType(type) ? _messages.ForbiddenDatabaseType(type.Name) : null;
    }

    #endregion
}
