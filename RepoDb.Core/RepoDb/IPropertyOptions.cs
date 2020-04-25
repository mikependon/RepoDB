using System.Data;

namespace RepoDb
{
    public interface IPropertyOptions<T> where T : class
    {
        IPropertyOptions<T> Column(string column);
        IPropertyOptions<T> DbType(DbType dbType);
        IPropertyOptions<T> PropertyHandler<THandler>(THandler propertyHandler);
    }
}