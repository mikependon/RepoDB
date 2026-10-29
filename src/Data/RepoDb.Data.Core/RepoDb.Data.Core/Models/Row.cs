#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;

namespace RepoDb.Data.Models
{
    /// <summary>
    /// A class that represents a row of the data being copied: its columns (see <see cref="Column"/>) and the value of each one. It is what an interceptor of the copy
    /// (<c>IDataInterceptor&lt;Row&gt;</c>) receives and returns: it can change the values, and add or remove columns. The names of the columns are matched without the case.
    /// </summary>
    public class Row
    {
        #region Private Variables

        private List<Column> _columns;
        private readonly List<object> _values;

        /// <summary>
        /// True while the list of the columns is the one that the rows of a copy share, so it is copied before the row adds or removes a column.
        /// </summary>
        private bool _shared;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="Row"/> class.
        /// </summary>
        /// <param name="columns">The columns of the row.</param>
        /// <param name="values">The values of the columns, in the same order.</param>
        /// <exception cref="ArgumentNullException">The columns or the values are <c>null</c>.</exception>
        /// <exception cref="ArgumentException">A column is <c>null</c>, or the number of values is not the number of columns.</exception>
        public Row(IEnumerable<Column> columns,
            IEnumerable<object> values)
        {
            if (columns == null)
            {
                throw new ArgumentNullException(nameof(columns));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            _columns = new List<Column>(columns);
            _values = new List<object>(values);
            if (_columns.Contains(null))
            {
                throw new ArgumentException("The columns must not contain a null item.",
                    nameof(columns));
            }

            if (_columns.Count != _values.Count)
            {
                throw new ArgumentException("The number of values must be the number of columns.",
                    nameof(values));
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="Row"/> class that shares the list of the columns with the other rows of the copy.
        /// </summary>
        /// <param name="columns">The columns of the row (shared, so they are copied if the row adds or removes a column).</param>
        /// <param name="values">The values of the columns, in the same order.</param>
        internal Row(List<Column> columns,
            object[] values)
        {
            _columns = columns;
            _values = new List<object>(values);
            _shared = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the columns of the row.
        /// </summary>
        public IReadOnlyList<Column> Columns => _columns;

        /// <summary>
        /// Gets the number of columns of the row.
        /// </summary>
        public int Count => _columns.Count;

        /// <summary>
        /// Gets the values of the columns, in the same order.
        /// </summary>
        public IReadOnlyList<object> Values => _values;

        /// <summary>
        /// Gets or sets the value of the column at the specified position.
        /// </summary>
        /// <param name="ordinal">The position of the column.</param>
        /// <returns>The value (<c>null</c> if it has none).</returns>
        /// <exception cref="ArgumentOutOfRangeException">The position is not the position of a column.</exception>
        public object this[int ordinal] { get => _values[ordinal]; set => _values[ordinal] = value; }

        /// <summary>
        /// Gets or sets the value of the column with the specified name (the case is ignored).
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The value (<c>null</c> if it has none).</returns>
        /// <exception cref="ArgumentException">The row has no column with the name.</exception>
        public object this[string name] { get => _values[GetOrdinal(name)]; set => _values[GetOrdinal(name)] = value; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the position of the column with the specified name (the case is ignored).
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The position of the column, or -1 if the row has no column with the name.</returns>
        public int IndexOf(string name)
        {
            for (var i = 0; i < _columns.Count; i++)
            {
                if (string.Equals(_columns[i].Name,
                    name,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Checks whether the row has a column with the specified name (the case is ignored).
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns><c>true</c> if the row has the column; otherwise, <c>false</c>.</returns>
        public bool Contains(string name) =>
            IndexOf(name) >= 0;

        /// <summary>
        /// Adds a column, at the end of the row.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="value">The value of the column.</param>
        /// <returns>The row itself.</returns>
        /// <exception cref="ArgumentNullException">The column is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The row has a column with the same name.</exception>
        public Row Add(Column column,
            object value)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }

            if (Contains(column.Name))
            {
                throw new ArgumentException($"The row already has the column '{column.Name}'.",
                    nameof(column));
            }

            Own();
            _columns.Add(column);
            _values.Add(value);
            return this;
        }

        /// <summary>
        /// Adds a column, at the end of the row.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <param name="type">The type of the column.</param>
        /// <param name="value">The value of the column.</param>
        /// <returns>The row itself.</returns>
        /// <exception cref="ArgumentException">The row has a column with the same name.</exception>
        public Row Add(string name,
            Type type,
            object value) =>
            Add(new Column(name,
                type),
                value);

        /// <summary>
        /// Removes the column with the specified name (the case is ignored), with its value.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns><c>true</c> if the column was removed; <c>false</c> if the row has no column with the name.</returns>
        public bool Remove(string name)
        {
            var ordinal = IndexOf(name);
            if (ordinal < 0)
            {
                return false;
            }

            Own();
            _columns.RemoveAt(ordinal);
            _values.RemoveAt(ordinal);
            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the position of the column with the specified name.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The position of the column.</returns>
        /// <exception cref="ArgumentException">The row has no column with the name.</exception>
        private int GetOrdinal(string name)
        {
            var ordinal = IndexOf(name);
            if (ordinal < 0)
            {
                throw new ArgumentException($"The row has no column '{name}'.",
                    nameof(name));
            }

            return ordinal;
        }

        /// <summary>
        /// Makes the list of the columns the row's own, so changing it does not change the other rows of the copy.
        /// </summary>
        private void Own()
        {
            if (_shared)
            {
                _columns = new List<Column>(_columns);
                _shared = false;
            }
        }

        #endregion
    }
}
