using Vertica.Data.VerticaClient;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize necessary objects that is connected to <see cref="VerticaConnection"/> object.
    /// </summary>
    public static class VerticaBootstrap
    {
        #region Properties

        /// <summary>
        /// Gets the value indicating whether the initialization is completed.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes all necessary settings for Vertica.
        /// </summary>
        [Obsolete("This class will soon to be hidden as internal class. Use the 'GlobalConfiguration.Setup().UseVertica()' method instead.")]
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
            DbSettingMapper.Add<VerticaConnection>(new VerticaDbSetting(), true);

            // Map the DbHelper
            DbHelperMapper.Add<VerticaConnection>(new VerticaDbHelper(), true);

            // Map the Statement Builder
            StatementBuilderMapper.Add<VerticaConnection>(new VerticaStatementBuilder(), true);

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
