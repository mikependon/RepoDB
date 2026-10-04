#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema.Enumerations
{
    /// <summary>
    /// An enumeration that is used to define which of the related tables (as defined by the foreign keys) are copied together with the requested tables.
    /// </summary>
    public enum CopySchemaRelationshipBehavior
    {
        /// <summary>
        /// Copies only the requested tables, without any of their relationships.
        /// </summary>
        TableOnly,

        /// <summary>
        /// Copies the requested tables and their parents: the tables that they reference through their foreign keys, and the tables that those reference, up to the top of the tree.
        /// </summary>
        Parents,

        /// <summary>
        /// Copies the requested tables and their children: the tables that reference them through their foreign keys, and the tables that reference those, down to the end of the tree.
        /// </summary>
        /// <remarks>
        /// The children can reference other tables that are not part of the copy. Their foreign keys can only be created if those tables already exist in the destination database.
        /// </remarks>
        Children,

        /// <summary>
        /// Copies all the tables that are connected to the requested tables, in any direction (their parents, their children, and the tables that those are related to), from the beginning of the tree until the end of it. It does not mean every table of the database.
        /// </summary>
        All
    }
}
