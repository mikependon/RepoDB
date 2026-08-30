#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using FirebirdSql.Data.FirebirdClient;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize necessary objects that is connected to <see cref="FbConnection"/> object.
    /// </summary>
    public static class FirebirdBootstrap
    {
        #region Properties

        /// <summary>
        /// Gets the value indicating whether the initialization is completed.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes all necessary settings for Firebird.
        /// </summary>
        [Obsolete("This class will soon to be hidden as internal class. Use the 'GlobalConfiguration.Setup().UseFirebird()' method instead.")]
        public static void Initialize() => InitializeInternal();

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
            DbSettingMapper.Add<FbConnection>(new FirebirdDbSetting(), true);

            // Map the DbHelper
            DbHelperMapper.Add<FbConnection>(new FirebirdDbHelper(), true);

            // Map the Statement Builder
            StatementBuilderMapper.Add<FbConnection>(new FirebirdStatementBuilder(), true);

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
