#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that orders the tables by the foreign keys that they have, and builds the relationships between them.
    /// It is shared by the schema readers, as only the way that a table is identified is different between the databases.
    /// </summary>
    internal static class CopySchemaOrder
    {
        #region Public Methods

        /// <summary>
        /// Orders the tables so that a table always comes after the tables that it references, and builds the parents and the children of each one.
        /// The tables that reference each other (directly or through other tables) have no order between them, so they are kept in the order that they were given.
        /// The tables that are not related to each other are also kept in the order that they were given.
        /// </summary>
        /// <param name="schemas">The schemas of the tables.</param>
        /// <param name="tableKey">Gets the key that identifies a table (the same table must always have the same key).</param>
        /// <param name="referenceKey">Gets the key of a table from the identity that a foreign key uses to reference it (see <see cref="ForeignKeyInfo.ReferencedTable"/>).</param>
        /// <returns>The ordered relationships, one per table.</returns>
        internal static IList<RelationshipInfo> Order(IList<TableSchema> schemas,
            Func<TableSchema, string> tableKey,
            Func<TableInfo, string> referenceKey)
        {
            var keys = schemas.Select(tableKey).Distinct().ToList();
            var relationships = CreateRelationships(schemas, tableKey);
            var position = keys.Select((key, index) => new { key, index }).ToDictionary(x => x.key, x => x.index);
            var parentKeys = GetParentKeys(keys, relationships, position, referenceKey);
            Link(keys, parentKeys, relationships);
            return OrderKeys(keys, parentKeys, position).Select(key => relationships[key]).ToList();
        }

        #endregion

        #region Helpers

        // Relationships

        /// <summary>
        /// Creates one relationship per table (the first one wins if a table is given more than once).
        /// </summary>
        /// <param name="schemas"></param>
        /// <param name="tableKey"></param>
        /// <returns></returns>
        private static Dictionary<string, RelationshipInfo> CreateRelationships(IEnumerable<TableSchema> schemas,
            Func<TableSchema, string> tableKey) =>
            schemas.GroupBy(tableKey).ToDictionary(g => g.Key, g => new RelationshipInfo { Schema = g.First() });

        /// <summary>
        /// Gets the parents of each table: the (other) given tables that its foreign keys reference.
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="relationships"></param>
        /// <param name="position"></param>
        /// <param name="referenceKey"></param>
        /// <returns></returns>
        private static Dictionary<string, List<string>> GetParentKeys(IEnumerable<string> keys,
            IDictionary<string, RelationshipInfo> relationships,
            IDictionary<string, int> position,
            Func<TableInfo, string> referenceKey) =>
            keys.ToDictionary(
                key => key,
                key => relationships[key].Schema.ForeignKeys
                    .Select(fk => referenceKey(fk.ReferencedTable))
                    .Where(parent => parent != key && relationships.ContainsKey(parent))
                    .Distinct()
                    .OrderBy(parent => position[parent])
                    .ToList());

        /// <summary>
        /// Links each relationship with its parents, and each parent with its children.
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="parentKeys"></param>
        /// <param name="relationships"></param>
        private static void Link(IList<string> keys,
            IDictionary<string, List<string>> parentKeys,
            IDictionary<string, RelationshipInfo> relationships)
        {
            foreach (var key in keys)
            {
                foreach (var parent in parentKeys[key])
                {
                    relationships[key].Parents.Add(relationships[parent]);
                }
            }
            foreach (var key in keys)
            {
                foreach (var parent in parentKeys[key])
                {
                    relationships[parent].Children.Add(relationships[key]);
                }
            }
        }

        // Ordering

        /// <summary>
        /// Orders the keys so that the tables of a group always come after the tables of the groups that they reference.
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="parentKeys"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private static IList<string> OrderKeys(IList<string> keys,
            IDictionary<string, List<string>> parentKeys,
            IDictionary<string, int> position)
        {
            var groupOf = FindGroups(keys, parentKeys);
            var groups = keys.GroupBy(key => groupOf[key]).ToDictionary(g => g.Key, g => g.ToList());
            var pending = GetPendingGroups(groups, parentKeys, groupOf);
            var ordered = new List<string>();
            while (pending.Count > 0)
            {
                var next = NextGroup(pending, groups, position);
                ordered.AddRange(groups[next].OrderBy(key => position[key]));
                pending.Remove(next);
                foreach (var parents in pending.Values)
                {
                    parents.Remove(next);
                }
            }
            return ordered;
        }

        /// <summary>
        /// Gets the groups that each group is still waiting for (the groups of the parents of its tables).
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="parentKeys"></param>
        /// <param name="groupOf"></param>
        /// <returns></returns>
        private static Dictionary<int, HashSet<int>> GetPendingGroups(IDictionary<int, List<string>> groups,
            IDictionary<string, List<string>> parentKeys,
            IDictionary<string, int> groupOf) =>
            groups.ToDictionary(
                group => group.Key,
                group => new HashSet<int>(group.Value
                    .SelectMany(key => parentKeys[key])
                    .Select(parent => groupOf[parent])
                    .Where(parent => parent != group.Key)));

        /// <summary>
        /// Gets the next group to take: the one that is not waiting for any group, and that has the table that was given first.
        /// </summary>
        /// <param name="pending"></param>
        /// <param name="groups"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private static int NextGroup(IDictionary<int, HashSet<int>> pending,
            IDictionary<int, List<string>> groups,
            IDictionary<string, int> position) =>
            pending
                .Where(p => p.Value.Count == 0)
                .OrderBy(p => groups[p.Key].Min(key => position[key]))
                .First()
                .Key;

        /// <summary>
        /// Finds the groups of the tables that can reach each other.
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="parentKeys"></param>
        /// <returns></returns>
        private static Dictionary<string, int> FindGroups(IEnumerable<string> keys,
            IDictionary<string, List<string>> parentKeys) =>
            new CopySchemaGroupFinder(parentKeys).Find(keys);

        #endregion
    }
}
