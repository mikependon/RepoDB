#region Copyright Attributions

// Copyright (c) 2022 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;
using RepoDb.DbSettings;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="SqlConnection"/> object.
    /// </summary>
    public static partial class SqlServerGlobalConfiguration
    {
        /// <summary>
        /// Initializes all the necessary settings for SQL Server.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseSqlServer(this GlobalConfiguration globalConfiguration)
        {
            SqlServerBootstrap.InitializeInternal();
            return globalConfiguration;
        }

        /// <summary>
        /// Initializes all the necessary settings for SQL Server, with the given setting. It accepts any class that inherits the
        /// <see cref="SqlServerDbSetting"/>, i.e. the SqlServerBulkOperationsDbSetting of RepoDb.SqlServer.BulkOperations. The given setting
        /// is always applied, even if SQL Server was already initialized.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <param name="dbSetting">The setting to be used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseSqlServer(this GlobalConfiguration globalConfiguration,
            SqlServerDbSetting dbSetting)
        {
            if (dbSetting == null)
            {
                throw new System.ArgumentNullException(nameof(dbSetting));
            }

            SqlServerBootstrap.InitializeInternal(dbSetting);
            return globalConfiguration;
        }
    }
}
