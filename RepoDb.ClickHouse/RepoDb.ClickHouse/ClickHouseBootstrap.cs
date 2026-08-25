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
    /// <remarks>
    /// RepoDb no longer owns a <c>ClickHouseConnection</c> subclass: every mapping registered here is anchored
    /// directly on <see cref="ClickHouseConnection"/> from <c>ClickHouse.Driver.ADO</c> itself.
    /// </remarks>
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
        /// <param name="isWaitForMutationsEnabled">A value indicating whether the internal mutations are enabled for the ClickHouse database.</param>
        internal static void InitializeInternal(bool isWaitForMutationsEnabled)
        {
            // Skip if already initialized
            if (IsInitialized == true)
            {
                return;
            }

            // Map the DbSetting
            var dbSetting = new ClickHouseDbSetting()
            {
                IsInternalMutationsEnabled = isWaitForMutationsEnabled
            };
            DbSettingMapper.Add<ClickHouseConnection>(dbSetting, true);

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
