using EnterpriseDB.EDBClient;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb
{
    /// <summary>
    /// A class used to initialize necessary objects that is connected to <see cref="EDBConnection"/> object.
    /// </summary>
    public static class EnterpriseDbBootstrap
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
            DbSettingMapper.Add<EDBConnection>(new EnterpriseDbDbSetting(), true);

            // Map the DbHelper
            DbHelperMapper.Add<EDBConnection>(new EnterpriseDbDbHelper(), true);

            // Map the Statement Builder
            StatementBuilderMapper.Add<EDBConnection>(new EnterpriseDbStatementBuilder(), true);

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
