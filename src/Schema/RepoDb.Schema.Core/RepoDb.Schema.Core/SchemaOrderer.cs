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
    internal static class SchemaOrderer
    {
        #region Public Methods

        /// <summary>
        /// Orders the tables so that a table always comes after the tables that it references, and builds the parents and the children of each one.
        /// The tables that reference each other (directly or through other tables) have no order between them, so they are kept in the order that they were given.
        /// The tables that are not related to each other are also kept in the order that they were given.
        /// </summary>
        /// <param name="schemas">The schemas of the tables.</param>
        /// <param name="tableKey">Gets the key that identifies a table (the same table must always have the same key).</param>
        /// <param name="referenceKey">Gets the key of a table from the name that a foreign key uses to reference it (see <see cref="ForeignKeyInfo.ReferencedTable"/>).</param>
        /// <returns>The ordered relationships, one per table.</returns>
        internal static IList<RelationshipInfo> Order(IList<TableSchema> schemas,
            Func<TableSchema, string> tableKey,
            Func<string, string> referenceKey)
        {
            // One relationship per table (the first one wins if a table is given more than once)
            var keys = new List<string>();
            var relationships = new Dictionary<string, RelationshipInfo>();
            foreach (var schema in schemas)
            {
                var key = tableKey(schema);
                if (!relationships.ContainsKey(key))
                {
                    keys.Add(key);
                    relationships[key] = new RelationshipInfo { Schema = schema };
                }
            }
            var position = keys.Select((key, index) => new { key, index }).ToDictionary(x => x.key, x => x.index);

            // The parents of each table: the (other) given tables that its foreign keys reference
            var parentKeys = keys.ToDictionary(
                key => key,
                key => relationships[key].Schema.ForeignKeys
                    .Select(fk => referenceKey(fk.ReferencedTable))
                    .Where(parent => parent != key && relationships.ContainsKey(parent))
                    .Distinct()
                    .OrderBy(parent => position[parent])
                    .ToList());
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

            // The tables that reference each other (directly or not) form a group, so find the groups first
            var groupOf = FindGroups(keys, parentKeys);
            var groups = keys.GroupBy(key => groupOf[key]).ToDictionary(g => g.Key, g => g.ToList());
            var pending = groups.ToDictionary(
                group => group.Key,
                group => new HashSet<int>(group.Value
                    .SelectMany(key => parentKeys[key])
                    .Select(parent => groupOf[parent])
                    .Where(parent => parent != group.Key)));

            // Take the groups, one by one, whose parent groups are all taken (the one with the table that was given first comes first)
            var ordered = new List<RelationshipInfo>();
            while (pending.Count > 0)
            {
                var next = pending
                    .Where(p => p.Value.Count == 0)
                    .OrderBy(p => groups[p.Key].Min(key => position[key]))
                    .First()
                    .Key;
                ordered.AddRange(groups[next].OrderBy(key => position[key]).Select(key => relationships[key]));
                pending.Remove(next);
                foreach (var parents in pending.Values)
                {
                    parents.Remove(next);
                }
            }
            return ordered;
        }

        #endregion

        #region Helpers

        /// <summary>
        ///
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="parentKeys"></param>
        /// <returns></returns>
        private static Dictionary<string, int> FindGroups(IList<string> keys,
            IDictionary<string, List<string>> parentKeys)
        {
            // Tarjan's algorithm: the tables that can reach each other belong to the same group
            var groupOf = new Dictionary<string, int>();
            var indexes = new Dictionary<string, int>();
            var lowLinks = new Dictionary<string, int>();
            var stack = new Stack<string>();
            var onStack = new HashSet<string>();
            var counter = 0;
            var groups = 0;

            void Visit(string key)
            {
                indexes[key] = lowLinks[key] = counter++;
                stack.Push(key);
                onStack.Add(key);
                foreach (var parent in parentKeys[key])
                {
                    if (!indexes.ContainsKey(parent))
                    {
                        Visit(parent);
                        lowLinks[key] = Math.Min(lowLinks[key], lowLinks[parent]);
                    }
                    else if (onStack.Contains(parent))
                    {
                        lowLinks[key] = Math.Min(lowLinks[key], indexes[parent]);
                    }
                }
                if (lowLinks[key] == indexes[key])
                {
                    string member;
                    do
                    {
                        member = stack.Pop();
                        onStack.Remove(member);
                        groupOf[member] = groups;
                    }
                    while (member != key);
                    groups++;
                }
            }

            foreach (var key in keys)
            {
                if (!indexes.ContainsKey(key))
                {
                    Visit(key);
                }
            }
            return groupOf;
        }

        #endregion
    }
}
