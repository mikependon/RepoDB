#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using DuckDB.NET.Data;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.PropertyHandlers.DuckDb;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize necessary objects that is connected to <see cref="DuckDBConnection"/> object.
    /// </summary>
    public static class DuckDbBootstrap
    {
        #region Properties

        /// <summary>
        /// Gets the value indicating whether the initialization is completed.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the necessary objects with the default <see cref="DuckDbDbSetting"/>. It is skipped if already initialized.
        /// </summary>
        /// <param name="addDefaultHandlers">
        /// A value indicating whether to map <see cref="DuckDbStreamToByteArrayPropertyHandler"/> and
        /// <see cref="DuckDbTimeOnlyToTimeSpanPropertyHandler"/> at the type level.
        /// </param>
        internal static void InitializeInternal(
            bool addDefaultHandlers = true) =>
            InitializeInternal(null, addDefaultHandlers);

        /// <summary>
        /// Initializes the necessary objects with the given setting. Unlike the parameterless call, a given setting is always
        /// applied (replacing the setting of an earlier initialization), as it is an explicit request of the caller.
        /// </summary>
        /// <param name="dbSetting">The setting to be used, or null to use the default <see cref="DuckDbDbSetting"/>.</param>
        /// <param name="addDefaultHandlers">
        internal static void InitializeInternal(DuckDbDbSetting dbSetting,
            bool addDefaultHandlers = true)
        {
            // Skip if already initialized (and no explicit setting was given)
            if (IsInitialized == true && dbSetting == null)
            {
                return;
            }

            // Map the DbSetting
            dbSetting ??= new DuckDbDbSetting();
            DbSettingMapper.Add<DuckDBConnection>(dbSetting, true);

            // Map the DbHelper
            DbHelperMapper.Add<DuckDBConnection>(new DuckDbDbHelper(), true);

            // Map the Statement Builder
            StatementBuilderMapper.Add<DuckDBConnection>(new DuckDbStatementBuilder(), true);

            // Map the PropertyHandlers if needed
            if (addDefaultHandlers)
            {
                PropertyHandlerMapper.Add<byte[], DuckDbStreamToByteArrayPropertyHandler>(true);
                PropertyHandlerMapper.Add<TimeSpan, DuckDbTimeOnlyToTimeSpanPropertyHandler>(true);
            }

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
