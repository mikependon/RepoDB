using System;
using System.Collections.Generic;
using RepoDb.Interfaces;
using System.Linq;
using System.Collections;

namespace RepoDb
{
    public class MemoryCache : ICache
    {
        private static object _syncLock = new object();
        private readonly IList<ICacheItem> _cacheList;

        internal MemoryCache()
        {
            _cacheList = new List<ICacheItem>();
        }

        public void Add(string key, object value)
        {
            lock (_syncLock)
            {
                var item = GetItem(key);
                if (item == null)
                {
                    _cacheList.Add(new CacheItem(key, value));
                }
                else
                {
                    var cacheItem = (CacheItem)item;
                    cacheItem.Value = value;
                    cacheItem.Timestamp = DateTime.UtcNow;
                }
            }
        }

        public void Add(ICacheItem item)
        {
            lock (_syncLock)
            {
                var cache = GetItem(item.Key);
                if (cache == null)
                {
                    _cacheList.Add(item);
                }
                else
                {
                    cache.Value = item.Value;
                    cache.Timestamp = DateTime.UtcNow;
                }
            }
        }

        public void Clear()
        {
            _cacheList.Clear();
        }

        public bool Contains(string key)
        {
            return GetItem(key) != null;
        }

        public object Get(string key)
        {
            var cacheItem = GetItem(key);
            return cacheItem?.Value;
        }

        public IEnumerator<ICacheItem> GetEnumerator()
        {
            return _cacheList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _cacheList.GetEnumerator();
        }

        private ICacheItem GetItem(string key)
        {
            return _cacheList.FirstOrDefault(cacheItem =>
                string.Equals(cacheItem.Key, key, StringComparison.CurrentCulture));
        }

        public void Remove(string key)
        {
            var item = GetItem(key);
            if (item != null)
            {
                _cacheList.Remove(item);
            }
        }
    }
}
