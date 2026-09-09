#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace RepoDb.Options;

/// <summary>
/// A class that holds the optional arguments for the 'Insert' operation.
/// </summary>
public class ConnectionInsertOptions
{
    /// <summary>
    /// Gets or sets the mapping list of <see cref="Field"/> objects to be used.
    /// </summary>
    public IEnumerable<Field> Fields { get; set; }

    /// <summary>
    /// Gets or sets the table hints to be used.
    /// </summary>
    public string Hints { get; set; }

    /// <summary>
    /// Gets or sets the command timeout in seconds to be used.
    /// </summary>
    public int? CommandTimeout { get; set; }

    /// <summary>
    /// Gets or sets the tracing key to be used.
    /// </summary>
    public string TraceKey { get; set; } = TraceKeys.Insert;

    /// <summary>
    /// Gets or sets the transaction to be used.
    /// </summary>
    public IDbTransaction Transaction { get; set; }

    /// <summary>
    /// Gets or sets the trace object to be used.
    /// </summary>
    public ITrace Trace { get; set; }

    /// <summary>
    /// Gets or sets the statement builder object to be used.
    /// </summary>
    public IStatementBuilder StatementBuilder { get; set; }
}
