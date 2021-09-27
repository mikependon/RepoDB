using Microsoft.Data.Sqlite;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.Resolvers;
using RepoDb.StatementBuilders;

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

            // Map the DbSetting
            var mdsDbSetting = new SqLiteDbSetting(false);
            DbSettingMapper.Add<SqliteConnection>(mdsDbSetting, true);

            // Map the DbHelper
            DbHelperMapper.Add<SqliteConnection>(new SqLiteDbHelper(mdsDbSetting, new MdsSqLiteDbTypeNameToClientTypeResolver()), true);

            // Map the Statement Builder
            StatementBuilderMapper.Add<SqliteConnection>(new SqLiteStatementBuilder(mdsDbSetting,
                new SqLiteConvertFieldResolver(),
                new ClientTypeToAverageableClientTypeResolver()), true);

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
