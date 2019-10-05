using RepoDb.DbHelpers;
using RepoDb.StatementBuilders;
using System.Data.SQLite;

namespace RepoDb.SqLite
{
    /// <summary>
    /// A class used to initialize necessary objects for <see cref=""/>
    /// </summary>
    public static class Initializer
    {

        public static void Initialize()
        {
            // Map the DbHelper
            DbHelperMapper.Add(typeof(SQLiteConnection), new SqLiteDbHelper(), true);

            // Map the Statement Builder
            StatementBuilderMapper.Add(typeof(SQLiteConnection), new SqLiteStatementBuilder(), true);
        }
    }
}
