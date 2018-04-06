using System.Collections.Generic;

namespace RepoDb.Interfaces
{
    public interface ICache
    {
        void Clear();
        IEnumerable<ICacheItem> GetAll();
        object Get(string key);
        void Set(string key, object value);
        bool Has(string key);
    }
}
