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
        /// Initializes the necessary objects with the default <see cref="AuroraDbDbSetting"/>. It is skipped if already initialized.
        /// </summary>
        internal static void InitializeInternal() =>
            InitializeInternal(null);

        /// <summary>
        /// Initializes the necessary objects with the given setting. Unlike the parameterless call, a given setting is always
        /// applied (replacing the setting of an earlier initialization), as it is an explicit request of the caller.
        /// </summary>
        /// <param name="dbSetting">The setting to be used, or null to use the default <see cref="AuroraDbDbSetting"/>.</param>
        internal static void InitializeInternal(AuroraDbDbSetting dbSetting)
        {
            // Skip if already initialized (and no explicit setting was given)
            if (IsInitialized == true && dbSetting == null)
            {
                return;
            }

            // Map the DbSetting
            dbSetting ??= new AuroraDbDbSetting();
            DbSettingMapper.Add<AuroraDbConnection>(dbSetting, true);

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
