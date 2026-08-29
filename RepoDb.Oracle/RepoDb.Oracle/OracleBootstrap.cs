#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
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
        ///
        /// </summary>
        internal static void InitializeInternal()
        {
            // Skip if already initialized
            if (IsInitialized == true)
            {
                return;
            }

            // ODP.NET defaults OracleCommand.BindByName to 'false' (positional binding), which is
            // incompatible with RepoDb's dynamically-generated, named bind variables. Force by-name
            // binding globally; must be set before any connections are opened.
            OracleConfiguration.BindByName = true;

            // Map the DbSetting
            var dbSetting = new OracleDbSetting();
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
