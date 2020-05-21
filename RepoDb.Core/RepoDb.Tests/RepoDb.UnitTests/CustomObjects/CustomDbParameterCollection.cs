using RepoDb.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace RepoDb.UnitTests.CustomObjects
{
    public class CustomDbParameterCollection : DbParameterCollection
    {
        #region Privates

        private List<object> m_parameters = new List<object>();

        #endregion

        #region Properties

        public override int Count => m_parameters.Count;

        public override object SyncRoot { get; }

        #endregion

        #region Methods

        public override int Add(object value)
        {
            m_parameters.Add(value);
            return m_parameters.IndexOf(value);
        }

        public new CustomDbParameter this[string name]
        {
            get
            {
                return m_parameters.OfType<CustomDbParameter>().First();
            }
        }

        public override void AddRange(Array values)
        {
            m_parameters.AddRange(values.AsEnumerable());
        }

        public override void Clear()
        {
            m_parameters.Clear();
        }

        public override bool Contains(object value)
        {
            return m_parameters.Contains(value);
        }

        public override bool Contains(string value)
        {
            return m_parameters
                .OfType<DbParameter>()
                .Any(p => string.Equals(p.ParameterName, value, StringComparison.OrdinalIgnoreCase));
        }

        public override void CopyTo(Array array,
            int index)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator GetEnumerator()
        {
            return m_parameters.GetEnumerator();
        }

        public override int IndexOf(object value)
        {
            return m_parameters.IndexOf(value);
        }

        public override int IndexOf(string parameterName)
        {
            var value = m_parameters
                .OfType<DbParameter>()
                .FirstOrDefault(p => string.Equals(p.ParameterName, parameterName, StringComparison.OrdinalIgnoreCase));
            return IndexOf(value);
        }

        public override void Insert(int index,
            object value)
        {
            m_parameters.Insert(index, value);
        }

        public override void Remove(object value)
        {
            m_parameters.Remove(value);
        }

        public override void RemoveAt(int index)
        {
            m_parameters.RemoveAt(index);
        }

        public override void RemoveAt(string parameterName)
        {
            var value = m_parameters
                .OfType<DbParameter>()
                .FirstOrDefault(p => string.Equals(p.ParameterName, parameterName, StringComparison.OrdinalIgnoreCase));
            Remove(value);
        }

        protected override DbParameter GetParameter(int index)
        {
            return m_parameters[index] as DbParameter;
        }

        protected override DbParameter GetParameter(string parameterName)
        {
            return m_parameters
                .OfType<DbParameter>()
                .FirstOrDefault(p => string.Equals(p.ParameterName, parameterName, StringComparison.OrdinalIgnoreCase));
        }

        protected override void SetParameter(int index,
            DbParameter value)
        {
            var parameter = GetParameter(index);
            parameter.Value = value;
        }

        protected override void SetParameter(string parameterName,
            DbParameter value)
        {
            var parameter = GetParameter(parameterName);
            parameter.Value = value;
        }

        #endregion
    }
}
