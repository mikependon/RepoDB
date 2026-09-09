#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data;

namespace RepoDb.Options;

/// <summary>
/// A class that holds the optional arguments for the 'Delete' operation of the <see cref="BaseRepository{TEntity, TDbConnection}"/> and <see cref="DbRepository{TDbConnection}"/> objects.
/// </summary>
public class RepositoryDeleteOptions
{
    /// <summary>
    /// Gets or sets the table hints to be used.
    /// </summary>
    public string Hints { get; set; }

    /// <summary>
    /// Gets or sets the tracing key to be used.
    /// </summary>
    public string TraceKey { get; set; } = TraceKeys.Delete;

    /// <summary>
    /// Gets or sets the transaction to be used.
    /// </summary>
    public IDbTransaction Transaction { get; set; }
}
