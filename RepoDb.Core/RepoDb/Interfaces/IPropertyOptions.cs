using System.Data;

namespace RepoDb.Interfaces
{
    public interface IPropertyOptions<T> where T : class
    {
        /*
         * Column
         */
        IPropertyOptions<T> Column(string column);

        /*
         * DbType
         */

        IPropertyOptions<T> DbType(DbType dbType);

        /*
         * PropertyHandler
         */

        IPropertyOptions<T> PropertyHandler<TPropertyHandler>();

        IPropertyOptions<T> PropertyHandler<TPropertyHandler>(bool force);

        IPropertyOptions<T> PropertyHandler<TPropertyHandler>(TPropertyHandler propertyHandler);

        IPropertyOptions<T> PropertyHandler<TPropertyHandler>(TPropertyHandler propertyHandler,
            bool force);
    }
}