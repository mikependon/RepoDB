using System.Data;
using System.Data.Common;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A static classed used to convert the <see cref="DbCommand"/> object.
    /// </summary>
    public static class DataCommand
    {
        public static void SetParameters<TEntity>(DbCommand command, TEntity entity, IDbConnection connection)
            where TEntity : class
        {

        }
    }
}
