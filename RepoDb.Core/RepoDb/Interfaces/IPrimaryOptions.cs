using System.Data;

namespace RepoDb.Interfaces
{
    public interface IPrimaryOptions<T> where T : class
    {
        /*
         * Column
         */

        IPrimaryOptions<T> Column(string column);
        
        IPrimaryOptions<T> Column(string column,
            bool force);

        /*
         * DbType
         */

        IPrimaryOptions<T> DbType(DbType dbType);

        IPrimaryOptions<T> DbType(DbType dbType,
            bool force);
    }
}