using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the default SQL executor against an in-memory SQLite database.
/// </summary>
internal static class NotebookSqlExecutorTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static async Task RunAsync()
    {
        ConstructorRejectsNullFactory();
        await ExecuteRejectsNullRequest();
        await ReadsColumnsWithTheirTypes();
        await ReadsValuesAsText();
        await ReadsEveryResultSet();
        await KeepsRowsUpToTheLimit();
        await MarksATruncatedTable();
        await BlocksWritesByDefault();
        await BlockedScriptOpensNoConnection();
        await AllowsWritesWhenAsked();
        await ReturnsNoTableForStatementsWithoutResults();
        await PassesTheConnectionId();
        await DisposesTheConnection();
        await AcceptsAnOpenConnection();
        await ReportsTheAffectedRows();
        await ReportsNoAffectedRowsForAQuery();
        await RefusesWithTheRequestMessages();
        await ReaderMarksNothingBelowTheLimit();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates an executor over a new in-memory database.
    /// </summary>
    /// <param name="allowWrites">Whether writes are allowed.</param>
    /// <returns>The executor.</returns>
    private static NotebookSqlExecutor Executor(bool allowWrites = false) => new((_,
            _) => ValueTask.FromResult<DbConnection>(new SqliteConnection("Data Source=:memory:")),
        allowWrites);

    /// <summary>
    /// Runs a script.
    /// </summary>
    /// <param name="script">The script.</param>
    /// <param name="rowLimit">The row limit.</param>
    /// <returns>The tables.</returns>
    private static async Task<IReadOnlyList<NotebookTable>> Run(string script,
        int rowLimit = 10)
    {
        var result = await Executor()
            .ExecuteAsync(new NotebookSqlRequest(script,
                null,
                rowLimit));
        return result.Tables;
    }

    /// <summary>
    /// Runs a script with writes allowed.
    /// </summary>
    /// <param name="script">The script.</param>
    /// <returns>The outcome.</returns>
    private static Task<NotebookSqlResult> RunWriting(string script) => Executor(true)
        .ExecuteAsync(new NotebookSqlRequest(script));

    /// <summary>
    /// Tests ConstructorRejectsNullFactory.
    /// </summary>
    private static void ConstructorRejectsNullFactory()
    {
        // Act
        Action operation = () => _ = new NotebookSqlExecutor(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests ExecuteRejectsNullRequest.
    /// </summary>
    private static async Task ExecuteRejectsNullRequest()
    {
        // Setup
        var executor = Executor();

        // Act/Assert
        await ThrowsAsync<ArgumentNullException>(() => executor.ExecuteAsync(null!));
    }

    /// <summary>
    /// Tests ReadsColumnsWithTheirTypes.
    /// </summary>
    private static async Task ReadsColumnsWithTheirTypes()
    {
        // Act
        var table = (await Run("SELECT 1 AS Id, 'a' AS Name, 1.5 AS Price"))
            .Single();

        // Assert
        Check(string.Join(',',
            table.Columns
                .Select(column => $"{column.Name}:{column.TypeName}")) == "Id:long,Name:string,Price:double");
    }

    /// <summary>
    /// Tests ReadsValuesAsText.
    /// </summary>
    private static async Task ReadsValuesAsText()
    {
        // Act
        var row = (await Run("SELECT 1, NULL, x'0102'"))
            .Single().Rows
            .Single();

        // Assert
        Check(row[0] == "1" && row[1] is null && row[2] == "AQI=");
    }

    /// <summary>
    /// Tests ReadsEveryResultSet.
    /// </summary>
    private static async Task ReadsEveryResultSet()
    {
        // Act
        var tables = await Run("SELECT 1; SELECT 2");

        // Assert
        Check(tables.Count == 2 && tables[1].Rows[0][0] == "2");
    }

    /// <summary>
    /// Tests KeepsRowsUpToTheLimit.
    /// </summary>
    private static async Task KeepsRowsUpToTheLimit()
    {
        // Act
        var table = (await Run("SELECT 1 UNION ALL SELECT 2",
            2))
            .Single();

        // Assert
        Check(table.Rows.Count == 2 && !table.IsTruncated);
    }

    /// <summary>
    /// Tests MarksATruncatedTable.
    /// </summary>
    private static async Task MarksATruncatedTable()
    {
        // Act
        var table = (await Run("SELECT 1 UNION ALL SELECT 2 UNION ALL SELECT 3",
            1))
            .Single();

        // Assert
        Check(table.Rows.Count == 1 && table.IsTruncated);
    }

    /// <summary>
    /// Tests BlocksWritesByDefault.
    /// </summary>
    private static async Task BlocksWritesByDefault()
    {
        // Act
        var error = await ThrowsAsync<InvalidOperationException>(() => Run("CREATE TABLE t (a)"));

        // Assert
        Check(error.Message == "Only SELECT statements can run because writes are not allowed.");
    }

    /// <summary>
    /// Tests BlockedScriptOpensNoConnection.
    /// </summary>
    private static async Task BlockedScriptOpensNoConnection()
    {
        // Setup
        var created = 0;
        var executor = new NotebookSqlExecutor((_,
            _) =>
        {
            created++;
            return ValueTask.FromResult<DbConnection>(new SqliteConnection("Data Source=:memory:"));
        });

        // Act
        await ThrowsAsync<InvalidOperationException>(() => executor.ExecuteAsync(new NotebookSqlRequest("DELETE FROM t")));

        // Assert
        Check(created == 0);
    }

    /// <summary>
    /// Tests AllowsWritesWhenAsked.
    /// </summary>
    private static async Task AllowsWritesWhenAsked()
    {
        // Act
        var result = await RunWriting("CREATE TABLE t (a); INSERT INTO t VALUES (7); SELECT a FROM t");

        // Assert
        Check(result.Tables
            .Single().Rows
            .Single()[0] == "7");
    }

    /// <summary>
    /// Tests ReturnsNoTableForStatementsWithoutResults.
    /// </summary>
    private static async Task ReturnsNoTableForStatementsWithoutResults()
    {
        // Act
        var result = await RunWriting("CREATE TABLE t (a)");

        // Assert
        Check(result.Tables.Count == 0 && result.Succeeded);
    }

    /// <summary>
    /// Tests ReportsTheAffectedRows.
    /// </summary>
    private static async Task ReportsTheAffectedRows()
    {
        // Act
        var result = await RunWriting("CREATE TABLE t (a); INSERT INTO t VALUES (1), (2)");

        // Assert
        Check(result.RowsAffected == 2);
    }

    /// <summary>
    /// Tests ReportsNoAffectedRowsForAQuery.
    /// </summary>
    private static async Task ReportsNoAffectedRowsForAQuery()
    {
        // Act
        var result = await Executor()
            .ExecuteAsync(new NotebookSqlRequest("SELECT 1"));

        // Assert
        Check(result.RowsAffected is null && result.Status is null);
    }

    /// <summary>
    /// Tests RefusesWithTheRequestMessages.
    /// </summary>
    private static async Task RefusesWithTheRequestMessages()
    {
        // Setup
        var request = new NotebookSqlRequest("DELETE FROM t") { Messages = new TagMessages() };

        // Act
        var error = await ThrowsAsync<InvalidOperationException>(() => Executor()
            .ExecuteAsync(request));

        // Assert
        Check(error.Message == "writes-not-allowed");
    }

    /// <summary>
    /// Tests PassesTheConnectionId.
    /// </summary>
    private static async Task PassesTheConnectionId()
    {
        // Setup
        var id = Guid.NewGuid();
        Guid? received = null;
        var executor = new NotebookSqlExecutor((connectionId,
            _) =>
        {
            received = connectionId;
            return ValueTask.FromResult<DbConnection>(new SqliteConnection("Data Source=:memory:"));
        });

        // Act
        await executor.ExecuteAsync(new NotebookSqlRequest("SELECT 1",
            id));

        // Assert
        Check(received == id);
    }

    /// <summary>
    /// Tests DisposesTheConnection.
    /// </summary>
    private static async Task DisposesTheConnection()
    {
        // Setup
        var connection = new SqliteConnection("Data Source=:memory:");
        var executor = new NotebookSqlExecutor((_,
            _) => ValueTask.FromResult<DbConnection>(connection));

        // Act
        await executor.ExecuteAsync(new NotebookSqlRequest("SELECT 1"));

        // Assert
        Check(connection.State == ConnectionState.Closed);
    }

    /// <summary>
    /// Tests AcceptsAnOpenConnection.
    /// </summary>
    private static async Task AcceptsAnOpenConnection()
    {
        // Setup
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var executor = new NotebookSqlExecutor((_,
            _) => ValueTask.FromResult<DbConnection>(connection));

        // Act
        var result = await executor.ExecuteAsync(new NotebookSqlRequest("SELECT 1"));

        // Assert
        Check(result.Tables.Count == 1);
    }

    /// <summary>
    /// Tests ReaderMarksNothingBelowTheLimit.
    /// </summary>
    private static async Task ReaderMarksNothingBelowTheLimit()
    {
        // Setup
        using var source = new DataTable();
        source.Columns.Add("Id",
            typeof(int));
        source.Rows.Add(1);
        using var reader = source.CreateDataReader();

        // Act
        var table = await NotebookTableReader.ReadAsync(reader,
            5,
            CancellationToken.None);

        // Assert
        Check(table is { Rows.Count: 1, IsTruncated: false } && table.Columns[0].TypeName == "int");
    }

    #endregion
}
