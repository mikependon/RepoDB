#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema.Enumerations
{
    /// <summary>
    /// An enumeration that is used to define what a schema copy has actually done to the table in the destination database.
    /// </summary>
    public enum CopySchemaOutcome
    {
        /// <summary>
        /// The table was created.
        /// </summary>
        Created,

        /// <summary>
        /// The table already existed and was left as is.
        /// </summary>
        Skipped,

        /// <summary>
        /// The table already existed and the missing columns and indexes were added.
        /// </summary>
        Aligned,

        /// <summary>
        /// The table already existed, was dropped and then re-created.
        /// </summary>
        Dropped
    }
}
