#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.AuroraDb.MySqlConnector;
using RepoDb.DbSettings;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="AuroraDbConnection"/> object.
    /// </summary>
    public static partial class AuroraDbGlobalConfiguration
    {
        /// <summary>
        /// Initializes all the necessary settings for AuroraDb.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseAuroraDb(
            this GlobalConfiguration globalConfiguration)
        {
            AuroraDbBootstrap.InitializeInternal();
            return globalConfiguration;
        }

        /// <summary>
        /// Initializes all the necessary settings for AuroraDB (MySQL), with the given setting. It accepts any class that inherits the
        /// <see cref="AuroraDbDbSetting"/>, i.e. the AuroraDbBulkOperationsDbSetting of RepoDb.AuroraDb.MySqlConnector.BulkOperations. The given
        /// setting is always applied, even if AuroraDB (MySQL) was already initialized.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <param name="dbSetting">The setting to be used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseAuroraDb(this GlobalConfiguration globalConfiguration,
            AuroraDbDbSetting dbSetting)
        {
            if (dbSetting == null)
            {
                throw new System.ArgumentNullException(nameof(dbSetting));
            }

            AuroraDbBootstrap.InitializeInternal(dbSetting);
            return globalConfiguration;
        }
    }
}
