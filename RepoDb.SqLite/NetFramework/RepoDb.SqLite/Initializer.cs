using RepoDb.DbHelpers;
using RepoDb.DbValidators;
using RepoDb.SqLite.DbSettings;
using RepoDb.StatementBuilders;
using System.Data.SQLite;

namespace RepoDb.SqLite
{
    /// <summary>
    /// A class used to initialize necessary objects that is connected to <see cref="SQLiteConnection"/> object.
    /// </summary>
    public static class Initializer
    {

        public static void Initialize()
        {
            // Map the Statement Builder
            StatementBuilderMapper.Add(typeof(SQLiteConnection), new SqLiteStatementBuilder(), true);

            // Map the DbHelper
            DbHelperMapper.Add(typeof(SQLiteConnection), new SqLiteDbHelper(), true);

            // Map the DbValidator
            DbValidatorMapper.Add(typeof(SQLiteConnection), new SqLiteDbValidator(), true);

            // Map the DbSetting
            DbSettingMapper.Add(typeof(SQLiteConnection), new SqLiteDbSetting(), true);

            // Map the DbOperation
            //DbSettingMapper.Add(typeof(SQLiteConnection), new SqLiteDbOperation(), true);
        }
    }
}
