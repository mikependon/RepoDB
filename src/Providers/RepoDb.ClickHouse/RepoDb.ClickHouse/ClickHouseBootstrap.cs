#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using ClickHouse.Driver.ADO;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.Interfaces;
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
        /// Initializes the necessary objects with the default <see cref="ClickHouseDbSetting"/>. It is skipped if already initialized.
        /// </summary>
        internal static void InitializeInternal() =>
            InitializeInternal(null);

        /// <summary>
        /// Initializes the necessary objects with the given setting. Unlike the parameterless call, a given setting is always
        /// applied (replacing the setting of an earlier initialization), as it is an explicit request of the caller.
        /// </summary>
        /// <param name="dbSetting">The setting to be used, or null to use the default <see cref="ClickHouseDbSetting"/>.</param>
        internal static void InitializeInternal(IDbSetting dbSetting)
        {
            // Skip if already initialized (and no explicit setting was given)
            if (IsInitialized == true && dbSetting == null)
            {
                return;
            }

            // Map the DbSetting
            dbSetting ??= new ClickHouseDbSetting();
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
