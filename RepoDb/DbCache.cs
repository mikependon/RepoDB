using System;
using System.Collections.Generic;
using RepoDb.Interfaces;
using System.Linq;

namespace RepoDb
{
    public sealed class DbCache : ICache
    {
        private static object _syncLock = new object();
        private readonly IList<ICacheItem> _list;

        public DbCache()
        {
            _list = new List<ICacheItem>();
        }

        public int MinutesLifespan { get; }

        public void Clear()
        {
            _list.Clear();
        }

        public object Get(string key)
        {
            var cacheItem = GetItem(key);
            return cacheItem?.Value;
        }

        public IEnumerable<ICacheItem> GetAll()
        {
            return _list;
        }

        public bool Has(string key)
        {
            return GetItem(key) != null;
        }

        public void Set(string key, object value)
        {
            lock (_syncLock)
            {
                var item = GetItem(key);
                if (item == null)
                {
                    _list.Add(new DbCacheItem(key, value));
                }
                else
                {
                    var cacheItem = (DbCacheItem)item;
                    cacheItem.Value = value;
                    cacheItem.Timestamp = DateTime.UtcNow;
                }
            }
        }

        private ICacheItem GetItem(string key)
        {
            return _list.FirstOrDefault(ci => string.Equals(ci.Key, key, StringComparison.CurrentCulture));
        }
    }
}
