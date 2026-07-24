using Oracle.ManagedDataAccess.Client;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="OracleConnection"/> object.
    /// </summary>
    public static class OracleBootstrap
    {
        #region Properties

        /// <summary>
        /// Gets the value that indicates whether the initialization is completed.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes all the necessary settings for Oracle.
        /// </summary>
        [Obsolete("This class will soon to be hidden as internal class. Use the 'GlobalConfiguration.Setup().UseOracle()' method instead.")]
        public static void Initialize() => InitializeInternal();

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
            var dbSetting = new OracleDbSetting();
            DbSettingMapper.Add<OracleConnection>(dbSetting, true);

            // Map the DbHelper
            var dbHelper = new OracleDbHelper();
            DbHelperMapper.Add<OracleConnection>(dbHelper, true);

            // Map the Statement Builder
            var statementBuilder = new OracleStatementBuilder(dbSetting);
            StatementBuilderMapper.Add<OracleConnection>(statementBuilder, true);

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
