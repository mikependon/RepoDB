#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using IBM.Data.Db2;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="DB2Connection"/> object.
    /// </summary>
    public static class Db2Bootstrap
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

            // Map the DbSetting
            var dbSetting = new Db2DbSetting();
            DbSettingMapper.Add<DB2Connection>(dbSetting, true);

            // Map the DbHelper
            var dbHelper = new Db2DbHelper();
            DbHelperMapper.Add<DB2Connection>(dbHelper, true);

            // Map the Statement Builder
            var statementBuilder = new Db2StatementBuilder(dbSetting);
            StatementBuilderMapper.Add<DB2Connection>(statementBuilder, true);

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
