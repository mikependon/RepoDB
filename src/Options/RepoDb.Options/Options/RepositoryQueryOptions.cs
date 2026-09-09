#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Data;

namespace RepoDb.Options;

/// <summary>
/// A class that holds the optional arguments for the 'Query' operation of the <see cref="BaseRepository{TEntity, TDbConnection}"/> and <see cref="DbRepository{TDbConnection}"/> objects.
/// </summary>
public class RepositoryQueryOptions
{
    /// <summary>
    /// Gets or sets the mapping list of <see cref="Field"/> objects to be used.
    /// </summary>
    public IEnumerable<Field> Fields { get; set; }

    /// <summary>
    /// Gets or sets the order definition of the fields to be used.
    /// </summary>
    public IEnumerable<OrderField> OrderBy { get; set; }

    /// <summary>
    /// Gets or sets the number of rows to be returned.
    /// </summary>
    public int? Top { get; set; } = 0;

    /// <summary>
    /// Gets or sets the table hints to be used.
    /// </summary>
    public string Hints { get; set; }

    /// <summary>
    /// Gets or sets the key to the cache item. By setting this property, it will return the item from the cache if present, otherwise it will query the database. This will only work if the repository has a cache object configured.
    /// </summary>
    public string CacheKey { get; set; }

    /// <summary>
    /// Gets or sets the tracing key to be used.
    /// </summary>
    public string TraceKey { get; set; } = TraceKeys.Query;

    /// <summary>
    /// Gets or sets the transaction to be used.
    /// </summary>
    public IDbTransaction Transaction { get; set; }
}
