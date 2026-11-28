namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// Members that reach a database through nullable types, for the scripts of the guard tests to name.
/// </summary>
public static class DatabaseUses
{
    #region Public Variables

    /// <summary>
    /// A field that holds a connection.
    /// </summary>
    public static readonly IDbConnection? Field;

    #endregion

    #region Properties

    /// <summary>
    /// Gets a property that holds a connection.
    /// </summary>
    public static IDbConnection? Property => Field;

    #endregion

    #region Public Methods

    /// <summary>
    /// A method that returns a connection.
    /// </summary>
    /// <returns>No connection.</returns>
    public static IDbConnection? Create() => Field;

    /// <summary>
    /// A method named as the type lookup that is not it.
    /// </summary>
    /// <param name="name">A name.</param>
    /// <returns>The name.</returns>
    public static string GetType(string name) => name;

    /// <summary>
    /// A method that takes a connection.
    /// </summary>
    /// <param name="connection">The connection.</param>
    public static void Use(IDbConnection? connection) => _ = connection;

    #endregion
}
