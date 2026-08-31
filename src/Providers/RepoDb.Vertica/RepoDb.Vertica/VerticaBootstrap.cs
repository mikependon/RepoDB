#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Vertica.Data.VerticaClient;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using System;
using System.Globalization;
using System.Threading;

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
        /// 
        /// </summary>
        /// <param name="useInvariantCulture"></param>
        internal static void InitializeInternal(
            bool useInvariantCulture = false)
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

            // Culture
            if (useInvariantCulture)
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            }

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
