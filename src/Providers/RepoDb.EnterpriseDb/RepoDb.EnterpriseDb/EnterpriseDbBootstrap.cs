#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using System;
using OfficialEDBConnection = EnterpriseDB.EDBClient.EDBConnection;
using ConnectorEDBConnection = RepoDb.Connector.EnterpriseDb.EDBConnection;

namespace RepoDb
{
    /// <summary>
    /// A class used to initialize necessary objects for EDB Postgres Advanced Server.
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
            DbSettingMapper.Add<OfficialEDBConnection>(new EnterpriseDbDbSetting(), true);
            DbSettingMapper.Add<ConnectorEDBConnection>(new EnterpriseDbDbSetting(), true);

            // Map the DbHelper
            DbHelperMapper.Add<OfficialEDBConnection>(new EnterpriseDbDbHelper(), true);
            DbHelperMapper.Add<ConnectorEDBConnection>(new EnterpriseDbDbHelper(), true);

            // Map the Statement Builder
            StatementBuilderMapper.Add<OfficialEDBConnection>(new EnterpriseDbStatementBuilder(), true);
            StatementBuilderMapper.Add<ConnectorEDBConnection>(new EnterpriseDbStatementBuilder(), true);

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
