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

            // The given tables first (the first one wins if a table is given more than once)
            var result = new List<TableInfo>();
            var seen = new HashSet<string>();
            foreach (var table in tables)
            {
                if (seen.Add(tableKey(table)))
                {
                    result.Add(table);
                }
            }
            if (relationshipBehavior == CopySchemaRelationshipBehavior.TableOnly)
            {
                return result;
            }

            // The neighbors of each table, by the key of the table
            var goUp = relationshipBehavior == CopySchemaRelationshipBehavior.Parents || relationshipBehavior == CopySchemaRelationshipBehavior.EndToEnd;
            var goDown = relationshipBehavior == CopySchemaRelationshipBehavior.Children || relationshipBehavior == CopySchemaRelationshipBehavior.EndToEnd;
            var neighbors = new Dictionary<string, List<TableInfo>>();
            foreach (var (child, parent) in foreignKeys)
            {
                var childKey = tableKey(child);
                var parentKey = tableKey(parent);
                if (childKey == parentKey)
                {
                    continue;
                }
                if (goUp)
                {
                    Add(neighbors, childKey, parent);
                }
                if (goDown)
                {
                    Add(neighbors, parentKey, child);
                }
            }

            // Breadth-first, so the nearest relatives come first and a cycle ends as soon as it comes back to a known table
            for (var i = 0; i < result.Count; i++)
            {
                if (!neighbors.TryGetValue(tableKey(result[i]), out var related))
                {
                    continue;
                }
                foreach (var table in related)
                {
                    if (seen.Add(tableKey(table)))
                    {
                        result.Add(table);
                    }
                }
            }
            return result;
        }

        #endregion

        #region Helpers

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
