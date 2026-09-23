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
        ///
        /// </summary>
        /// <param name="addDefaultHandlers">
        /// A value indicating whether to map <see cref="DuckDbStreamToByteArrayPropertyHandler"/> and
        /// <see cref="DuckDbTimeOnlyToTimeSpanPropertyHandler"/> at the type level.
        /// </param>
        internal static void InitializeInternal(
            bool addDefaultHandlers = true)
        {
            // Skip if already initialized
            if (IsInitialized == true)
            {
                return;
            }

            // Map the DbSetting
            DbSettingMapper.Add<DuckDBConnection>(new DuckDbDbSetting(), true);

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
