using MySqlConnector;
using RepoDb.DbHelpers;
using RepoDb.MySqlConnector.DbSettings;
using RepoDb.StatementBuilders;

namespace RepoDb
{
    /// <summary>
    /// A class used to initialize necessary objects that is connected to <see cref="MySqlConnection"/> object.
    /// </summary>
    public static class MySqlConnectorBootstrap
    {
        #region Properties

        /// <summary>
        /// Gets the value indicating whether the initialization is completed.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes all necessary settings for MySql.
        /// </summary>
        public static void Initialize()
        {
            // Skip if already initialized
            if (IsInitialized == true)
            {
                return;
            }

            // Map the DbSetting
            DbSettingMapper.Add(typeof(MySqlConnection), new MySqlConnectorDbSetting(), true);

            // Map the DbHelper
            DbHelperMapper.Add(typeof(MySqlConnection), new MySqlConnectorDbHelper(), true);

            // Map the Statement Builder
            StatementBuilderMapper.Add(typeof(MySqlConnection), new MySqlConnectorStatementBuilder(), true);

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
