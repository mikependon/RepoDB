#region Copyright Attributions

// Copyright (c) 2022 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using MySql.Data.MySqlClient;
using RepoDb.DbSettings;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="MySqlConnection"/> object.
    /// </summary>
    public static partial class MySqlGlobalConfiguration
    {
        /// <summary>
        /// Initializes all the necessary settings for MySQL.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseMySql(this GlobalConfiguration globalConfiguration)
        {
            MySqlBootstrap.InitializeInternal();
            return globalConfiguration;
        }

        /// <summary>
        /// Initializes all the necessary settings for MySQL, with the given setting. It accepts any class that inherits the
        /// <see cref="MySqlDbSetting"/>, i.e. the MySqlBulkOperationsDbSetting of RepoDb.MySql.BulkOperations. The given
        /// setting is always applied, even if MySQL was already initialized.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <param name="dbSetting">The setting to be used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseMySql(this GlobalConfiguration globalConfiguration,
            MySqlDbSetting dbSetting)
        {
            if (dbSetting == null)
            {
                throw new System.ArgumentNullException(nameof(dbSetting));
            }

            MySqlBootstrap.InitializeInternal(dbSetting);
            return globalConfiguration;
        }
    }
}
