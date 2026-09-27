#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;

namespace RepoDb
{
    /// <summary>
    /// A class used to initialize necessary objects for Aurora PostgreSQL.
    /// </summary>
    public static class AuroraDbBootstrap
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
            DbSettingMapper.Add<AuroraDbConnection>(new AuroraDbDbSetting(), true);

            // Map the DbHelper
            DbHelperMapper.Add<AuroraDbConnection>(new AuroraDbDbHelper(), true);

            // Map the Statement Builder
            StatementBuilderMapper.Add<AuroraDbConnection>(new AuroraDbStatementBuilder(), true);

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
