using RepoDb.Interfaces;

namespace RepoDb
{
    public class DataEntity : IDataEntity
    {
        public DataEntity() { }

        public object GetValue(string property)
        {
            return this.GetType()
                .GetProperty(property)
                .GetValue(this);
        }

        public T GetValue<T>(string property)
        {
            return (T)GetValue(property);
        }
    }
}
