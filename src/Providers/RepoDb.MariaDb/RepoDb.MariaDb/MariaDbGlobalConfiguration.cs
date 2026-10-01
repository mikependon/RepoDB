#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.MariaDb;
using RepoDb.DbSettings;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="MariaDbConnection"/> object.
    /// </summary>
    public static partial class MariaDbGlobalConfiguration
    {
        /// <summary>
        /// Initializes all the necessary settings for MariaDb.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseMariaDb(this GlobalConfiguration globalConfiguration)
        {
            MariaDbBootstrap.InitializeInternal();
            return globalConfiguration;
        }

        /// <summary>
        /// Initializes all the necessary settings for MariaDB, with the given setting. It accepts any class that inherits the
        /// <see cref="MariaDbDbSetting"/>, i.e. the MariaDbBulkOperationsDbSetting of RepoDb.MariaDb.BulkOperations. The given
        /// setting is always applied, even if MariaDB was already initialized.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <param name="dbSetting">The setting to be used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseMariaDb(this GlobalConfiguration globalConfiguration,
            MariaDbDbSetting dbSetting)
        {
            if (dbSetting == null)
            {
                throw new System.ArgumentNullException(nameof(dbSetting));
            }

            MariaDbBootstrap.InitializeInternal(dbSetting);
            return globalConfiguration;
        }
    }
}
