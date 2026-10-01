#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Oracle.ManagedDataAccess.Client;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="OracleConnection"/> object.
    /// </summary>
    public static class OracleBootstrap
    {
        #region Properties

        /// <summary>
        /// Gets the value that indicates whether the initialization is completed.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the necessary objects with the default <see cref="OracleDbSetting"/>. It is skipped if already initialized.
        /// </summary>
        internal static void InitializeInternal() =>
            InitializeInternal(null);

        /// <summary>
        /// Initializes the necessary objects with the given setting. Unlike the parameterless call, a given setting is always
        /// applied (replacing the setting of an earlier initialization), as it is an explicit request of the caller.
        /// </summary>
        /// <param name="dbSetting">The setting to be used, or null to use the default <see cref="OracleDbSetting"/>.</param>
        internal static void InitializeInternal(OracleDbSetting dbSetting)
        {
            // Skip if already initialized (and no explicit setting was given)
            if (IsInitialized == true && dbSetting == null)
            {
                return;
            }

            // ODP.NET defaults OracleCommand.BindByName to 'false' (positional binding), which is
            // incompatible with RepoDb's dynamically-generated, named bind variables. Force by-name
            // binding globally; must be set before any connections are opened. It is only set once, as ODP.NET
            // rejects it after a connection was opened (i.e. when an explicit setting is given later on).
            if (IsInitialized == false)
            {
                OracleConfiguration.BindByName = true;
            }

            // Map the DbSetting
            dbSetting ??= new OracleDbSetting();
            DbSettingMapper.Add<OracleConnection>(dbSetting, true);

            // Map the DbHelper
            var dbHelper = new OracleDbHelper();
            DbHelperMapper.Add<OracleConnection>(dbHelper, true);

            // Map the Statement Builder
            var statementBuilder = new OracleStatementBuilder(dbSetting);
            StatementBuilderMapper.Add<OracleConnection>(statementBuilder, true);

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
