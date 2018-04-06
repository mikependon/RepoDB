using System;
using System.Collections.Generic;
using RepoDb.Interfaces;
using System.Linq;

namespace RepoDb
{
    public sealed class DbCache : ICache
    {
        private readonly IList<ICacheItem> _list;

        internal DbCache()
        {
            _list = new List<ICacheItem>();
        }

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
            var cacheItem = GetItem(key);
            if (cacheItem == null)
            {
                _list.Add(new DbCacheItem(key, value));
            }
            else
            {
                ((DbCacheItem)cacheItem).Value = value;
            }
        }

        private ICacheItem GetItem(string key)
        {
            return _list.FirstOrDefault(ci => string.Equals(ci.Key, key, StringComparison.CurrentCulture));
        }
    }
}
