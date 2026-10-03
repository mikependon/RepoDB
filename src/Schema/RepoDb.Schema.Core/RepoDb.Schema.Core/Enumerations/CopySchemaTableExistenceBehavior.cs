#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema.Enumerations
{
    /// <summary>
    /// An enumeration that is used to define how the copy of a schema behaves when the table already exists in the destination database.
    /// </summary>
    public enum CopySchemaTableExistenceBehavior
    {
        /// <summary>
        /// Skips the copy if the table is already existing.
        /// </summary>
        SkipOnExists,

        /// <summary>
        /// If the table is already existing, only the missing columns and indexes are added.
        /// </summary>
        AlignOnExists,

        /// <summary>
        /// Throws an error if the table is already existing.
        /// </summary>
        ThrowOnExists,

        /// <summary>
        /// Drops the existing table if present and then proceeds with the creation.
        /// </summary>
        /// <remarks>
        /// <b>WARNING:</b> This permanently deletes the existing table together with all of its data in the destination database.
        /// This cannot be undone. Use this only when you are certain the destination table can be discarded.
        /// </remarks>
        DropOnExists
    }
}
