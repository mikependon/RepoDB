#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="SqlConnection"/> object.
    /// </summary>
    public static class SqlServerBootstrap
    {
        #region Properties

        /// <summary>
        /// Gets the value that indicates whether the initialization is completed.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the necessary objects with the default <see cref="SqlServerDbSetting"/>. It is skipped if already initialized.
        /// </summary>
        internal static void InitializeInternal() =>
            InitializeInternal(null);

        /// <summary>
        /// Initializes the necessary objects with the given setting. Unlike the parameterless call, a given setting is always
        /// applied (replacing the setting of an earlier initialization), as it is an explicit request of the caller.
        /// </summary>
        /// <param name="dbSetting">The setting to be used, or null to use the default <see cref="SqlServerDbSetting"/>.</param>
        internal static void InitializeInternal(SqlServerDbSetting dbSetting)
        {
            // Skip if already initialized (and no explicit setting was given)
            if (IsInitialized && dbSetting == null)
            {
                return;
            }

            // Map the DbSetting
            dbSetting ??= new SqlServerDbSetting();
            DbSettingMapper.Add<SqlConnection>(dbSetting, force: true);

            // Map the DbHelper
            var dbHelper = new SqlServerDbHelper();
            DbHelperMapper.Add<SqlConnection>(dbHelper, force: true);

            // Map the Statement Builder
            var statementBuilder = new SqlServerStatementBuilder(dbSetting);
            StatementBuilderMapper.Add<SqlConnection>(statementBuilder, force: true);

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
