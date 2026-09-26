#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.CockroachDb;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;

namespace RepoDb
{
    /// <summary>
    /// A class used to initialize necessary objects for CockroachDB.
    /// </summary>
    public static class CockroachDbBootstrap
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
            DbSettingMapper.Add<CockroachDbConnection>(new CockroachDbDbSetting(), true);

            // Map the DbHelper
            DbHelperMapper.Add<CockroachDbConnection>(new CockroachDbDbHelper(), true);

            // Map the Statement Builder
            StatementBuilderMapper.Add<CockroachDbConnection>(new CockroachDbStatementBuilder(), true);

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
