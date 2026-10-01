#region Copyright Attributions

// Copyright (c) 2022 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Npgsql;
using RepoDb.DbSettings;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="NpgsqlConnection"/> object.
    /// </summary>
    public static partial class PostgreSqlGlobalConfiguration
    {
        /// <summary>
        /// Initializes all the necessary settings for PostgreSql.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UsePostgreSql(this GlobalConfiguration globalConfiguration)
        {
            PostgreSqlBootstrap.InitializeInternal();
            return globalConfiguration;
        }

        /// <summary>
        /// Initializes all the necessary settings for PostgreSql, with the given setting. It accepts any class that inherits the
        /// <see cref="PostgreSqlDbSetting"/>, i.e. the PostgreSqlBulkOperationsDbSetting of RepoDb.PostgreSql.BulkOperations. The given
        /// setting is always applied, even if PostgreSql was already initialized.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <param name="dbSetting">The setting to be used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UsePostgreSql(this GlobalConfiguration globalConfiguration,
            PostgreSqlDbSetting dbSetting)
        {
            if (dbSetting == null)
            {
                throw new System.ArgumentNullException(nameof(dbSetting));
            }

            PostgreSqlBootstrap.InitializeInternal(dbSetting);
            return globalConfiguration;
        }
    }
}
