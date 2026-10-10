#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace RepoDb.Data.Core.UnitTests.CustomObjects
{
    public class FakeDbParameterCollection : DbParameterCollection
    {
        private readonly List<DbParameter> _parameters = new List<DbParameter>();

        public override int Count => _parameters.Count;

        public override object SyncRoot => ((ICollection)_parameters).SyncRoot;

        public override int Add(object value)
        {
            _parameters.Add((DbParameter)value);
            return _parameters.Count - 1;
        }

        public override void AddRange(Array values)
        {
            _parameters.AddRange(values.Cast<DbParameter>());
        }

        public override void Clear() =>
            _parameters.Clear();

        public override bool Contains(object value) =>
            _parameters.Contains((DbParameter)value);

        public override bool Contains(string value) =>
            IndexOf(value) >= 0;

        public override void CopyTo(Array array,
            int index) =>
            ((ICollection)_parameters).CopyTo(array, index);

        public override IEnumerator GetEnumerator() =>
            _parameters.GetEnumerator();

        public override int IndexOf(object value) =>
            _parameters.IndexOf((DbParameter)value);

        public override int IndexOf(string parameterName) =>
            _parameters.FindIndex(p => string.Equals(p.ParameterName, parameterName, StringComparison.OrdinalIgnoreCase));

        public override void Insert(int index,
            object value) =>
            _parameters.Insert(index, (DbParameter)value);

        public override void Remove(object value) =>
            _parameters.Remove((DbParameter)value);

        public override void RemoveAt(int index) =>
            _parameters.RemoveAt(index);

        public override void RemoveAt(string parameterName) =>
            _parameters.RemoveAt(IndexOf(parameterName));

        protected override DbParameter GetParameter(int index) =>
            _parameters[index];

        protected override DbParameter GetParameter(string parameterName) =>
            _parameters[IndexOf(parameterName)];

        protected override void SetParameter(int index,
            DbParameter value) =>
            _parameters[index] = value;

        protected override void SetParameter(string parameterName,
            DbParameter value) =>
            _parameters[IndexOf(parameterName)] = value;
    }
}
