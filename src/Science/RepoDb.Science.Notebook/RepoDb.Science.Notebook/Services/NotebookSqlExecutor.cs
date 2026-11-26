using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using RepoDb.Science.Helpers;
using RepoDb.Science.Services;

namespace RepoDb.Science;

/// <summary>
/// Runs SQL with ADO.NET. Unless writes are allowed, it only runs scripts that look like plain queries.
/// This check is a safeguard and not a security boundary: use a read-only database user to enforce it.
/// </summary>
public sealed class NotebookSqlExecutor : INotebookSqlExecutor
{
    #region Private Variables

    private readonly bool _allowWrites;
    private readonly Func<Guid?, CancellationToken, ValueTask<DbConnection>> _connectionFactory;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an executor.
    /// </summary>
    /// <param name="connectionFactory">Creates the connection of an identifier. The executor disposes it.</param>
    /// <param name="allowWrites">Whether scripts that change data may run.</param>
    public NotebookSqlExecutor(Func<Guid?, CancellationToken, ValueTask<DbConnection>> connectionFactory,
        bool allowWrites = false)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
        _allowWrites = allowWrites;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Runs a SQL script.
    /// </summary>
    /// <param name="request">The script to run.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result tables.</returns>
    public async Task<IReadOnlyList<NotebookTable>> ExecuteAsync(NotebookSqlRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        EnsureAllowed(request.Script);
        var connection = await _connectionFactory(request.ConnectionId,
            cancellationToken)
            .ConfigureAwait(false);
        await using var scope = connection.ConfigureAwait(false);
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        return await ReadAsync(connection,
            request,
            cancellationToken)
            .ConfigureAwait(false);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Rejects a script that may write when writes are not allowed.
    /// </summary>
    /// <param name="script">The script.</param>
    private void EnsureAllowed(string script)
    {
        if (!_allowWrites && !NotebookSqlReadOnlyCheck.IsReadOnly(script))
        {
            throw new InvalidOperationException("Only SELECT statements can run because writes are not allowed.");
        }
    }

    /// <summary>
    /// Runs the script and reads every result set.
    /// </summary>
    /// <param name="connection">The open connection.</param>
    /// <param name="request">The script to run.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result tables.</returns>
    [SuppressMessage("Security",
        "CA2100:Review SQL queries for security vulnerabilities",
        Justification = "Running the script the user wrote is the purpose of a notebook.")]
    private static async Task<IReadOnlyList<NotebookTable>> ReadAsync(DbConnection connection,
        NotebookSqlRequest request,
        CancellationToken cancellationToken)
    {
        var command = connection.CreateCommand();
        await using var commandScope = command.ConfigureAwait(false);
        command.CommandText = request.Script;
        var reader = await command.ExecuteReaderAsync(cancellationToken)
            .ConfigureAwait(false);
        await using var readerScope = reader.ConfigureAwait(false);
        var tables = new List<NotebookTable>();
        do
        {
            if (reader.FieldCount > 0)
            {
                tables.Add(await NotebookTableReader.ReadAsync(reader,
                    request.RowLimit,
                    cancellationToken)
                    .ConfigureAwait(false));
            }
        }
        while (await reader.NextResultAsync(cancellationToken)
            .ConfigureAwait(false));
        return tables;
    }

    #endregion
}
