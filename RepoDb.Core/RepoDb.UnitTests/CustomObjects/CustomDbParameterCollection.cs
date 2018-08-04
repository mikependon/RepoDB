using System;
using System.Collections;
using System.Data.Common;

namespace RepoDb.UnitTests.CustomObjects
{
    public class CustomDbParameterCollection : DbParameterCollection
    {
        public override int Count { get; }
        public override object SyncRoot { get; }
        public override int Add(object value)
        {
            return default(int);
        }
        public override void AddRange(Array values)
        {
        }
        public override void Clear()
        {
        }
        public override bool Contains(object value)
        {
            return default(bool);
        }
        public override bool Contains(string value)
        {
            return default(bool);
        }
        public override void CopyTo(Array array, int index)
        {
        }
        public override IEnumerator GetEnumerator()
        {
            return null;
        }
        public override int IndexOf(object value)
        {
            return default(int);
        }
        public override int IndexOf(string parameterName)
        {
            return default(int);
        }
        public override void Insert(int index, object value)
        {
        }
        public override void Remove(object value)
        {
        }
        public override void RemoveAt(int index)
        {
        }
        public override void RemoveAt(string parameterName)
        {
        }
        protected override DbParameter GetParameter(int index)
        {
            return default(DbParameter);
        }
        protected override DbParameter GetParameter(string parameterName)
        {
            return default(DbParameter);
        }
        protected override void SetParameter(int index, DbParameter value)
        {
        }
        protected override void SetParameter(string parameterName, DbParameter value)
        {
        }
    }

}
