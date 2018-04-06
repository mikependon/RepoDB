using System;
using RepoDb.Interfaces;

namespace RepoDb
{
    public sealed class DbCacheItem : ICacheItem
    {
        internal DbCacheItem()
        {
        }

        internal DbCacheItem(string key, object value)
        {
            Key = key;
            Value = value;
            Timestamp = DateTime.UtcNow;
        }

        public string Key { get; internal set; }

        public object Value { get; internal set; }

        public DateTime Timestamp { get; internal set; }
    }
}