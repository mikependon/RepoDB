#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using MySql.Data.MySqlClient;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;

namespace RepoDb
{
    /// <summary>
    /// A class used to initialize necessary objects that is connected to <see cref="MySqlConnection"/> object.
    /// </summary>
    public static class MySqlBootstrap
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
            DbSettingMapper.Add<MySqlConnection>(new MySqlDbSetting(), true);

            // Map the DbHelper
            DbHelperMapper.Add<MySqlConnection>(new MySqlDbHelper(), true);

            // Map the Statement Builder
            StatementBuilderMapper.Add<MySqlConnection>(new MySqlStatementBuilder(), true);

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
