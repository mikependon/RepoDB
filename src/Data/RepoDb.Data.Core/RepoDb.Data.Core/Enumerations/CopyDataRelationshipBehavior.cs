#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Data.Enumerations
{
    /// <summary>
    /// An enumeration that is used to define the data of which related tables (as defined by the foreign keys) is copied together with the data of the requested tables.
    /// </summary>
    public enum CopyDataRelationshipBehavior
    {
        /// <summary>
        /// Copies only the data of the requested tables, without the data of any of their relationships.
        /// </summary>
        TableOnly,

        /// <summary>
        /// Copies the data of the requested tables and of their parents: the tables that they reference through their foreign keys, and the tables that those reference, up to the top of the tree.
        /// </summary>
        Parents,

        /// <summary>
        /// Copies the data of the requested tables and of their children: the tables that reference them through their foreign keys, and the tables that reference those, down to the end of the tree.
        /// </summary>
        /// <remarks>
        /// The rows of the children can reference rows of other tables whose data is not part of the copy. Those rows can only be copied if the referenced rows already exist in the destination database.
        /// </remarks>
        Children,

        /// <summary>
        /// Copies the data of the requested tables together with the data of their parents and their children, and of the tables that those are related to in turn, in any direction: from the beginning of the tree until the end of it. Only the data of the tables that are connected to the requested tables is copied, not the data of every table of the database.
        /// </summary>
        ParentsAndChildren
    }
}
