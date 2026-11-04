using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

namespace RepoDb.Schema.UnitTestHelpers
{
    /// <summary>
    /// Provides an in-memory catalog test fixture.
    /// </summary>
    public sealed class CatalogParameterCollection : DbParameterCollection
    {
        #region Private Variables

        private readonly List<DbParameter> _items = new List<DbParameter>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of parameters.
        /// </summary>
        public override int Count => _items.Count;

        /// <summary>
        /// Gets the synchronization object.
        /// </summary>
        public override object SyncRoot => ((ICollection)_items).SyncRoot;

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a parameter.
        /// </summary>
        /// <param name="value">The value value.</param>
        public override int Add(object value)
        {
            _items.Add((DbParameter)value);
            return _items.Count - 1;
        }

        /// <summary>
        /// Adds an array of parameters.
        /// </summary>
        /// <param name="values">The values value.</param>
        public override void AddRange(Array values)
        {
            foreach (var value in values)
                Add(value);
        }

        /// <summary>
        /// Clears the parameters.
        /// </summary>
        public override void Clear() => _items.Clear();

        /// <summary>
        /// Checks a parameter instance.
        /// </summary>
        /// <param name="value">The value value.</param>
        public override bool Contains(object value) => _items.Contains((DbParameter)value);

        /// <summary>
        /// Checks a parameter name.
        /// </summary>
        /// <param name="value">The value value.</param>
        public override bool Contains(string value) => IndexOf(value) >= 0;

        /// <summary>
        /// Copies the parameters.
        /// </summary>
        /// <param name="array">The array value.</param>
        /// <param name="index">The index value.</param>
        public override void CopyTo(Array array,
            int index) => ((ICollection)_items).CopyTo(array,
                index);

        /// <summary>
        /// Enumerates the parameters.
        /// </summary>
        public override IEnumerator GetEnumerator() => _items.GetEnumerator();

        /// <summary>
        /// Finds a parameter instance.
        /// </summary>
        /// <param name="value">The value value.</param>
        public override int IndexOf(object value) => _items.IndexOf((DbParameter)value);

        /// <summary>
        /// Finds a parameter by name.
        /// </summary>
        /// <param name="parameterName">The parameterName value.</param>
        public override int IndexOf(string parameterName) =>
            _items.FindIndex(item => string.Equals(item.ParameterName,
                parameterName,
                StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Inserts a parameter.
        /// </summary>
        /// <param name="index">The index value.</param>
        /// <param name="value">The value value.</param>
        public override void Insert(int index,
            object value) => _items.Insert(index,
                (DbParameter)value);

        /// <summary>
        /// Removes a parameter.
        /// </summary>
        /// <param name="value">The value value.</param>
        public override void Remove(object value) => _items.Remove((DbParameter)value);

        /// <summary>
        /// Removes a parameter by position.
        /// </summary>
        /// <param name="index">The index value.</param>
        public override void RemoveAt(int index) => _items.RemoveAt(index);

        /// <summary>
        /// Removes a parameter by name.
        /// </summary>
        /// <param name="parameterName">The parameterName value.</param>
        public override void RemoveAt(string parameterName) => _items.RemoveAt(IndexOf(parameterName));

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets a parameter by position.
        /// </summary>
        /// <param name="index">The index value.</param>
        protected override DbParameter GetParameter(int index) => _items[index];

        /// <summary>
        /// Gets a parameter by name.
        /// </summary>
        /// <param name="parameterName">The parameterName value.</param>
        protected override DbParameter GetParameter(string parameterName) => _items[IndexOf(parameterName)];

        /// <summary>
        /// Sets a parameter by position.
        /// </summary>
        /// <param name="index">The index value.</param>
        /// <param name="value">The value value.</param>
        protected override void SetParameter(int index,
            DbParameter value) => _items[index] = value;

        /// <summary>
        /// Sets a parameter by name.
        /// </summary>
        /// <param name="parameterName">The parameterName value.</param>
        /// <param name="value">The value value.</param>
        protected override void SetParameter(string parameterName,
            DbParameter value) => _items[IndexOf(parameterName)] = value;

        #endregion
    }
}
