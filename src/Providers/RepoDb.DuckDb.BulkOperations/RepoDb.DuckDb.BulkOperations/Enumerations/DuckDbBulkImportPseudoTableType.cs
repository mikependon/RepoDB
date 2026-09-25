#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Enumerations.DuckDb
{
    /// <summary>
    /// Defines the kind of staging table used by the DuckDB bulk operations.
    /// </summary>
    public enum DuckDbBulkImportPseudoTableType : short
    {
        /// <summary>
        /// Uses <see cref="Memory"/>. This is the default.
        /// </summary>
        Auto,
        /// <summary>
        /// Uses a session-private temporary table.
        /// </summary>
        Memory,
        /// <summary>
        /// Uses an ordinary table.
        /// </summary>
        Physical
    }
}
