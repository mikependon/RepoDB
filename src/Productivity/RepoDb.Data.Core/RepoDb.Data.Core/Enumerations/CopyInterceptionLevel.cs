#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Data
{
    /// <summary>
    /// An enumeration that is used to define the level at which the data is intercepted while being copied.
    /// </summary>
    public enum CopyInterceptionLevel
    {
        /// <summary>
        /// Intercepts the data row by row. The data is streamed and not buffered, so this is the lowest in memory requirements.
        /// </summary>
        Row,

        /// <summary>
        /// Intercepts the data as a whole table. Note that the memory requirements are higher as the data are buffered within memory.
        /// </summary>
        Table,

        /// <summary>
        /// Intercepts the data row by row and then as a whole table. Note that the memory requirements are higher as the data are buffered within memory.
        /// </summary>
        RowAndTable
    }
}
