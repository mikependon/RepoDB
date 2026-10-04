#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that expands a set of tables with the tables that are related to them, as defined by the foreign keys.
    /// It is shared by the schema readers, as only the way that a table is identified is different between the databases.
    /// </summary>
    internal static class RelationshipExpander
    {
        #region Public Methods

        /// <summary>
        /// Expands the tables with their parents, their children or both (see <see cref="CopySchemaRelationshipBehavior"/>).
        /// </summary>
        /// <param name="tables">The tables to be expanded.</param>
        /// <param name="foreignKeys">The foreign key relationships of the database. A relationship is a table (the child) that references another table (the parent).</param>
        /// <param name="relationshipBehavior">The relationships to be followed.</param>
        /// <param name="tableKey">Gets the key that identifies a table (the same table must always have the same key).</param>
        /// <returns>
        /// The given tables in the order that they were given (without the duplicates), followed by the related tables
        /// in the order that they were found (nearest first).
        /// </returns>
        internal static IList<TableInfo> Expand(IEnumerable<TableInfo> tables,
            IEnumerable<(TableInfo Child, TableInfo Parent)> foreignKeys,
            CopySchemaRelationshipBehavior relationshipBehavior,
            Func<TableInfo, string> tableKey)
        {
            if (!Enum.IsDefined(typeof(CopySchemaRelationshipBehavior), relationshipBehavior))
            {
                throw new ArgumentOutOfRangeException(nameof(relationshipBehavior));
            }

            var result = tables.GroupBy(tableKey).Select(g => g.First()).ToList();
            return relationshipBehavior == CopySchemaRelationshipBehavior.TableOnly
                ? result
                : Walk(result, GetNeighbors(foreignKeys, relationshipBehavior, tableKey), tableKey);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets the neighbors of each table, by the key of the table: its parents and/or its children, depending on the behavior.
        /// A table that references itself is not a neighbor of itself.
        /// </summary>
        /// <param name="foreignKeys"></param>
        /// <param name="relationshipBehavior"></param>
        /// <param name="tableKey"></param>
        /// <returns></returns>
        private static Dictionary<string, List<TableInfo>> GetNeighbors(IEnumerable<(TableInfo Child, TableInfo Parent)> foreignKeys,
            CopySchemaRelationshipBehavior relationshipBehavior,
            Func<TableInfo, string> tableKey)
        {
            var neighbors = new Dictionary<string, List<TableInfo>>();
            foreach (var (child, parent) in foreignKeys.Where(fk => tableKey(fk.Child) != tableKey(fk.Parent)))
            {
                if (FollowsParents(relationshipBehavior))
                {
                    Add(neighbors, tableKey(child), parent);
                }
                if (FollowsChildren(relationshipBehavior))
                {
                    Add(neighbors, tableKey(parent), child);
                }
            }
            return neighbors;
        }

        /// <summary>
        /// Adds the neighbors of the tables that are already in the list, and then the neighbors of those, until no new table is found.
        /// It is breadth-first, so the nearest relatives come first and a cycle ends as soon as it comes back to a known table.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="neighbors"></param>
        /// <param name="tableKey"></param>
        /// <returns></returns>
        private static IList<TableInfo> Walk(List<TableInfo> result,
            IDictionary<string, List<TableInfo>> neighbors,
            Func<TableInfo, string> tableKey)
        {
            var seen = new HashSet<string>(result.Select(tableKey));
            for (var i = 0; i < result.Count; i++)
            {
                var related = neighbors.TryGetValue(tableKey(result[i]), out var list) ? list : Enumerable.Empty<TableInfo>();
                result.AddRange(related.Where(table => seen.Add(tableKey(table))).ToList());
            }
            return result;
        }

        /// <summary>
        /// Checks whether the behavior follows the parents of a table.
        /// </summary>
        /// <param name="relationshipBehavior"></param>
        /// <returns></returns>
        private static bool FollowsParents(CopySchemaRelationshipBehavior relationshipBehavior) =>
            relationshipBehavior == CopySchemaRelationshipBehavior.Parents ||
            relationshipBehavior == CopySchemaRelationshipBehavior.ParentsAndChildren;

        /// <summary>
        /// Checks whether the behavior follows the children of a table.
        /// </summary>
        /// <param name="relationshipBehavior"></param>
        /// <returns></returns>
        private static bool FollowsChildren(CopySchemaRelationshipBehavior relationshipBehavior) =>
            relationshipBehavior == CopySchemaRelationshipBehavior.Children ||
            relationshipBehavior == CopySchemaRelationshipBehavior.ParentsAndChildren;

        /// <summary>
        ///
        /// </summary>
        /// <param name="neighbors"></param>
        /// <param name="key"></param>
        /// <param name="table"></param>
        private static void Add(IDictionary<string, List<TableInfo>> neighbors,
            string key,
            TableInfo table)
        {
            if (!neighbors.TryGetValue(key, out var list))
            {
                neighbors[key] = list = new List<TableInfo>();
            }
            list.Add(table);
        }

        #endregion
    }
}
