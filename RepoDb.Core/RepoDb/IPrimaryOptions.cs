using System.Data;

namespace RepoDb
{
    public interface IPrimaryOptions<T> where T : class
    {
        IPrimaryOptions<T> Column(string column);
        IPrimaryOptions<T> DbType(DbType dbType);
    }
}