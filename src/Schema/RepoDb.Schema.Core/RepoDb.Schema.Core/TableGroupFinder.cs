#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that finds the groups of the tables that can reach each other, through their foreign keys (Tarjan's algorithm).
    /// </summary>
    internal sealed class TableGroupFinder
    {
        #region Private Variables

        private readonly IDictionary<string, List<string>> _parentKeys;
        private readonly Dictionary<string, int> _groupOf = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _indexes = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _lowLinks = new Dictionary<string, int>();
        private readonly Stack<string> _stack = new Stack<string>();
        private readonly HashSet<string> _onStack = new HashSet<string>();
        private int _counter;
        private int _groups;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="TableGroupFinder"/> class.
        /// </summary>
        /// <param name="parentKeys">The keys of the parents of each table, by the key of the table.</param>
        public TableGroupFinder(IDictionary<string, List<string>> parentKeys)
        {
            _parentKeys = parentKeys;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the group of each table.
        /// </summary>
        /// <param name="keys">The keys of the tables.</param>
        /// <returns>The group of each table, by the key of the table.</returns>
        public Dictionary<string, int> Find(IEnumerable<string> keys)
        {
            foreach (var key in keys.Where(key => !_indexes.ContainsKey(key)))
            {
                Visit(key);
            }

            return _groupOf;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Visits the table that is identified by the key, and the tables that it references (the depth-first search of the algorithm).
        /// </summary>
        /// <param name="key">The key of the table.</param>
        private void Visit(string key)
        {
            _indexes[key] = _lowLinks[key] = _counter++;
            _stack.Push(key);
            _onStack.Add(key);
            foreach (var parent in _parentKeys[key])
            {
                VisitParent(key,
                    parent);
            }

            if (_lowLinks[key] == _indexes[key])
            {
                TakeGroup(key);
            }
        }

        /// <summary>
        /// Visits a table that is referenced by another table, and updates the low link of the table that references it.
        /// </summary>
        /// <param name="key">The key of the table that references.</param>
        /// <param name="parent">The key of the table that is referenced.</param>
        private void VisitParent(string key,
            string parent)
        {
            if (!_indexes.ContainsKey(parent))
            {
                Visit(parent);
                _lowLinks[key] = Math.Min(_lowLinks[key],
                    _lowLinks[parent]);
            }
            else if (_onStack.Contains(parent))
            {
                _lowLinks[key] = Math.Min(_lowLinks[key],
                    _indexes[parent]);
            }
        }

        /// <summary>
        /// Takes the tables of a group off the stack, up to the root of the group.
        /// </summary>
        /// <param name="root">The key of the root table of the group.</param>
        private void TakeGroup(string root)
        {
            string member;
            do
            {
                member = _stack.Pop();
                _onStack.Remove(member);
                _groupOf[member] = _groups;
            }
            while (member != root);
            _groups++;
        }

        #endregion
    }
}
