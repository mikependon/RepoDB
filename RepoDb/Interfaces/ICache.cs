using System.Collections.Generic;

namespace RepoDb.Interfaces
{
    public interface ICache
    {
        void Add(string key, object value);
        void Add(ICacheItem item);
        void Clear();
        object Get(string key);
        IEnumerable<ICacheItem> GetAll();
        bool Has(string key);
        void Remove(string key);
    }
}
