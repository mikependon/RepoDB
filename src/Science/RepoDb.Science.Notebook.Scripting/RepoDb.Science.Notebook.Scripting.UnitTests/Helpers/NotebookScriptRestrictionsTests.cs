using Microsoft.CodeAnalysis;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the reasons of what a script may not use.
/// </summary>
internal static class NotebookScriptRestrictionsTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        ConstructorRejectsNullMessages();
        NoSymbolHasNoReason();
        ANamespaceHasNoReason();
        ATypeWithoutANamespaceHasNoReason();
        ABlockedTypeHasAReason();
        ADatabaseInterfaceHasAReason();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Gets why a symbol may not be used.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <returns>The reason, or null.</returns>
    private static string? ReasonOf(ISymbol? symbol) => new NotebookScriptRestrictions(NotebookScriptingMessages.Default)
        .ReasonOf(symbol);

    /// <summary>
    /// Gets a type the scripts see.
    /// </summary>
    /// <param name="name">The full name.</param>
    /// <returns>The symbol.</returns>
    private static INamedTypeSymbol TypeOf(string name) => Compilations.Of("1")
        .GetTypeByMetadataName(name)!;

    /// <summary>
    /// Tests ConstructorRejectsNullMessages.
    /// </summary>
    private static void ConstructorRejectsNullMessages()
    {
        // Act
        Action operation = () => _ = new NotebookScriptRestrictions(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests NoSymbolHasNoReason.
    /// </summary>
    private static void NoSymbolHasNoReason()
    {
        // Act
        var reason = ReasonOf(null);

        // Assert
        Check(reason is null);
    }

    /// <summary>
    /// Tests ANamespaceHasNoReason.
    /// </summary>
    private static void ANamespaceHasNoReason()
    {
        // Setup
        var space = TypeOf("Microsoft.Data.Sqlite.SqliteConnection").ContainingNamespace;

        // Act
        var reason = ReasonOf(space);

        // Assert
        Check(reason is null);
    }

    /// <summary>
    /// Tests ATypeWithoutANamespaceHasNoReason.
    /// </summary>
    private static void ATypeWithoutANamespaceHasNoReason()
    {
        // Setup
        var compilation = Compilations.Of("1");
        var pointer = compilation.CreatePointerTypeSymbol(compilation.GetSpecialType(SpecialType.System_Int32));

        // Act
        var reason = ReasonOf(pointer);

        // Assert
        Check(reason is null);
    }

    /// <summary>
    /// Tests ABlockedTypeHasAReason.
    /// </summary>
    private static void ABlockedTypeHasAReason()
    {
        // Act
        var reason = ReasonOf(TypeOf("System.Activator"));

        // Assert
        Check(reason == "System.Activator is not allowed");
    }

    /// <summary>
    /// Tests ADatabaseInterfaceHasAReason.
    /// </summary>
    private static void ADatabaseInterfaceHasAReason()
    {
        // Act
        var reason = ReasonOf(TypeOf("System.Data.IDbTransaction"));

        // Assert
        Check(reason == "IDbTransaction is a database connection or command");
    }

    #endregion
}
