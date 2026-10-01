#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.DbSettings;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the Oracle data provider.
    /// </summary>
    public static partial class OracleGlobalConfiguration
    {
        /// <summary>
        /// Initializes all the necessary settings for Oracle.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseOracle(this GlobalConfiguration globalConfiguration)
        {
            OracleBootstrap.InitializeInternal();
            return globalConfiguration;
        }

        /// <summary>
        /// Initializes all the necessary settings for Oracle, with the given setting. It accepts any class that inherits the
        /// <see cref="OracleDbSetting"/>, i.e. the OracleBulkOperationsDbSetting of RepoDb.Oracle.BulkOperations. The given
        /// setting is always applied, even if Oracle was already initialized.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <param name="dbSetting">The setting to be used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseOracle(this GlobalConfiguration globalConfiguration,
            OracleDbSetting dbSetting)
        {
            if (dbSetting == null)
            {
                throw new System.ArgumentNullException(nameof(dbSetting));
            }

            OracleBootstrap.InitializeInternal(dbSetting);
            return globalConfiguration;
        }
    }
}
