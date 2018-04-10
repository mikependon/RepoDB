using System.Collections.Generic;

namespace RepoDb.Interfaces
{
    public interface ICache : IEnumerable<ICacheItem>
    {
        void Add(string key, object value);
        void Add(ICacheItem item);
        void Clear();
        bool Contains(string key);
        object Get(string key);
        void Remove(string key);
    }
}
