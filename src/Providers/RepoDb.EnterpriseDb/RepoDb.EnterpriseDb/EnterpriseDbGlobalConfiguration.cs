#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.EnterpriseDb;
using RepoDb.DbSettings;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="EDBConnection"/> object.
    /// </summary>
    public static partial class EnterpriseDbGlobalConfiguration
    {
        /// <summary>
        /// Initializes all the necessary settings for EnterpriseDB (EDB Postgres Advanced Server).
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseEnterpriseDb(this GlobalConfiguration globalConfiguration)
        {
            EnterpriseDbBootstrap.InitializeInternal();
            return globalConfiguration;
        }

        /// <summary>
        /// Initializes all the necessary settings for EnterpriseDB, with the given setting. It accepts any class that inherits the
        /// <see cref="EnterpriseDbDbSetting"/>, i.e. the EDBBulkOperationsDbSetting of RepoDb.EnterpriseDb.BulkOperations. The given
        /// setting is always applied, even if EnterpriseDB was already initialized.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <param name="dbSetting">The setting to be used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseEnterpriseDb(this GlobalConfiguration globalConfiguration,
            EnterpriseDbDbSetting dbSetting)
        {
            if (dbSetting == null)
            {
                throw new System.ArgumentNullException(nameof(dbSetting));
            }

            EnterpriseDbBootstrap.InitializeInternal(dbSetting);
            return globalConfiguration;
        }
    }
}
