using Microsoft.Data.Sqlite;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
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
            DbSettingMapper.Add(typeof(SqliteConnection), new SqLiteDbSetting(), true);

            // Map the DbHelper
            DbHelperMapper.Add(typeof(SqliteConnection), new SqLiteDbHelper(), true);

            // Map the Statement Builder
            StatementBuilderMapper.Add(typeof(SqliteConnection), new SqLiteStatementBuilder(), true);

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
