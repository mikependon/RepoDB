using System.Data;

namespace RepoDb.Interfaces
{
    public interface IPrimaryOptions<T> where T : class
    {
        IPrimaryOptions<T> Column(string column);
        IPrimaryOptions<T> DbType(DbType dbType);
    }
}