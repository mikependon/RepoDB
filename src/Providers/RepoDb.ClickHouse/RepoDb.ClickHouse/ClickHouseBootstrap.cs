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
        ///
        /// </summary>
        internal static void InitializeInternal()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        internal static void InitializeInternal(IDbSetting dbSetting)
        {
            // Skip if already initialized
            if (IsInitialized == true)
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
