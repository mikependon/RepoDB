using Microsoft.Data.Sqlite;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.Resolvers;
using RepoDb.StatementBuilders;
using System.Data.SQLite;

namespace RepoDb
{
    /// <summary>
    /// A class used to initialize necessary objects that is connected to <see cref="SqliteConnection"/> object.
    /// </summary>
    public static class SqLiteBootstrap
    {
        #region Properties

        /// <summary>
        /// Gets the value indicating whether the initialization is completed.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes all necessary settings for SqLite.
        /// </summary>
        public static void Initialize()
        {
            // Skip if already initialized
            if (IsInitialized == true)
            {
                return;
            }

            #region SDS

            // Map the DbSetting
            DbSettingMapper.Add(typeof(SQLiteConnection), new SqLiteDbSetting(true), true);

            // Map the DbHelper
            DbHelperMapper.Add(typeof(SQLiteConnection), new SqLiteDbHelper(new SqLiteDbTypeNameToClientTypeResolver()), true);

            // Map the Statement Builder
            StatementBuilderMapper.Add(typeof(SQLiteConnection), new SqLiteStatementBuilder(DbSettingMapper.Get(typeof(SQLiteConnection)),
                new SqLiteConvertFieldResolver(),
                new ClientTypeToAverageableClientTypeResolver()), true);

            #endregion

            #region MDS

            // Map the DbSetting
            DbSettingMapper.Add(typeof(SqliteConnection), new SqLiteDbSetting(false), true);

            // Map the DbHelper
            DbHelperMapper.Add(typeof(SqliteConnection), new SqLiteDbHelper(new MdsSqLiteDbTypeNameToClientTypeResolver()), true);

            // Map the Statement Builder
            StatementBuilderMapper.Add(typeof(SqliteConnection), new SqLiteStatementBuilder(DbSettingMapper.Get(typeof(SqliteConnection)),
                new SqLiteConvertFieldResolver(),
                new ClientTypeToAverageableClientTypeResolver()), true);

            #endregion

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
