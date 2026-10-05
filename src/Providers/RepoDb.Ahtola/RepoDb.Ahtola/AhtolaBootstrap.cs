#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Ahtola.Data.Sqlite;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.Resolvers;
using RepoDb.StatementBuilders;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize necessary objects that is connected to <see cref="SqliteConnection"/> object.
    /// </summary>
    public static class AhtolaBootstrap
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
            var mdsDbSetting = new AhtolaDbSetting(false);
            DbSettingMapper.Add<SqliteConnection>(mdsDbSetting, true);

            // Map the DbHelper
            DbHelperMapper.Add<SqliteConnection>(new AhtolaDbHelper(mdsDbSetting, new AhtolaDbTypeNameToClientTypeResolver()), true);

            // Map the Statement Builder
            StatementBuilderMapper.Add<SqliteConnection>(new AhtolaStatementBuilder(mdsDbSetting,
                new AhtolaConvertFieldResolver(),
                new ClientTypeToAverageableClientTypeResolver()), true);

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
