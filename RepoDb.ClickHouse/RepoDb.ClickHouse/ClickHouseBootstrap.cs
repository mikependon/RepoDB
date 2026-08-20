using ClickHouse.Driver.ADO;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb
{
    /// <summary>
    /// A class used to initialize necessary objects that is connected to <see cref="ClickHouseConnection"/> object.
    /// </summary>
    public static class ClickHouseBootstrap
    {
        #region Properties

        /// <summary>
        /// Gets the value indicating whether the initialization is completed.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///
        /// </summary>
        internal static void InitializeInternal()
        {
            // Skip if already initialized
            if (IsInitialized == true)
            {
                return;
            }

            // Map the DbSetting
            DbSettingMapper.Add<ClickHouseConnection>(new ClickHouseDbSetting(), true);

            // Map the DbHelper
            DbHelperMapper.Add<ClickHouseConnection>(new ClickHouseDbHelper(), true);

            // Map the Statement Builder
            StatementBuilderMapper.Add<ClickHouseConnection>(new ClickHouseStatementBuilder(), true);

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
