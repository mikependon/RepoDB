#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Sap.Data.Hana;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.Interfaces;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize necessary objects that is connected to <see cref="HanaConnection"/> object.
    /// </summary>
    public static class SapHanaBootstrap
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
        /// <param name="dbSetting"></param>
        internal static void InitializeInternal(IDbSetting dbSetting)
        {
            // Skip if already initialized
            if (IsInitialized == true)
            {
                return;
            }

            // Map the DbSetting
            dbSetting ??= new SapHanaDbSetting();
            DbSettingMapper.Add<HanaConnection>(dbSetting, true);

            // Map the DbHelper
            DbHelperMapper.Add<HanaConnection>(new SapHanaDbHelper(), true);

            // Map the Statement Builder
            StatementBuilderMapper.Add<HanaConnection>(new SapHanaStatementBuilder(), true);

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
