using RepoDb.Interfaces;

namespace RepoDb
{
    public sealed class DbCacheItem : ICacheItem
    {
        private DbCacheItem()
        {
        }

        internal DbCacheItem(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; internal set; }

        public object Value { get; internal set; }
    }
}
