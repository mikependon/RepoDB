#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using DuckDB.NET.Data;
using RepoDb.DbSettings;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="DuckDBConnection"/> object.
    /// </summary>
    public static partial class DuckDbConfiguration
    {
        /// <summary>
        /// Initializes all the necessary settings for DuckDB.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseDuckDb(this GlobalConfiguration globalConfiguration)
        {
            UseDuckDb(globalConfiguration, true);
            return globalConfiguration;
        }

        /// <summary>
        /// Initializes all the necessary settings for DuckDB.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <param name="addDefaultHandlers">
        /// A value indicating whether to map the default DuckDB-specific <c>PropertyHandler</c>s at the type level.
        /// </param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseDuckDb(this GlobalConfiguration globalConfiguration,
            bool addDefaultHandlers)
        {
            DuckDbBootstrap.InitializeInternal(addDefaultHandlers);
            return globalConfiguration;
        }

        /// <summary>
        /// Initializes all the necessary settings for DuckDB, with the given setting. It accepts any class that inherits the
        /// <see cref="DuckDbDbSetting"/>, i.e. the DuckDbBulkOperationsDbSetting of RepoDb.DuckDb.BulkOperations. The given
        /// setting is always applied, even if DuckDB was already initialized.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <param name="dbSetting">The setting to be used.</param>
        /// <param name="addDefaultHandlers">
        /// A value indicating whether to map the default DuckDB-specific <c>PropertyHandler</c>s at the type level.
        /// </param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseDuckDb(this GlobalConfiguration globalConfiguration,
            DuckDbDbSetting dbSetting,
            bool addDefaultHandlers = true)
        {
            if (dbSetting == null)
            {
                throw new System.ArgumentNullException(nameof(dbSetting));
            }

            DuckDbBootstrap.InitializeInternal(dbSetting, addDefaultHandlers);
            return globalConfiguration;
        }
    }
}
