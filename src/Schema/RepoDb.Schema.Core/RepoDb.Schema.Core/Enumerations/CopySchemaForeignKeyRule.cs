#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema.Enumerations
{
    /// <summary>
    /// An enumeration that is used to define the rule applied by a foreign key when the referenced row is updated or deleted.
    /// </summary>
    public enum CopySchemaForeignKeyRule
    {
        /// <summary>
        /// No action is taken and the operation fails if it violates the foreign key.
        /// </summary>
        NoAction,

        /// <summary>
        /// The operation is restricted if it violates the foreign key.
        /// </summary>
        Restrict,

        /// <summary>
        /// The change is propagated to the referencing rows.
        /// </summary>
        Cascade,

        /// <summary>
        /// The referencing columns are set to <c>NULL</c>.
        /// </summary>
        SetNull,

        /// <summary>
        /// The referencing columns are set to their default values.
        /// </summary>
        SetDefault
    }
}
