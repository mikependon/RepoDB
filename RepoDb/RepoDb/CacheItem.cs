using System;
using RepoDb.Interfaces;

namespace RepoDb
{
    public class CacheItem : ICacheItem
    {
        public CacheItem(string key, object value)
        {
            Key = key;
            Value = value;
            Timestamp = DateTime.UtcNow;
        }

        public string Key { get; }

        public object Value { get; set; }

        public DateTime Timestamp { get; set; }
    }
}