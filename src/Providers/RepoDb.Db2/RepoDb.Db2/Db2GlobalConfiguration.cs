#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.DbSettings;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the Db2 data provider.
    /// </summary>
    public static partial class Db2GlobalConfiguration
    {
        /// <summary>
        /// Initializes all the necessary settings for Db2.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseDb2(this GlobalConfiguration globalConfiguration)
        {
            Db2Bootstrap.InitializeInternal();
            return globalConfiguration;
        }

        /// <summary>
        /// Initializes all the necessary settings for DB2, with the given setting. It accepts any class that inherits the
        /// <see cref="Db2DbSetting"/>, i.e. the Db2BulkOperationsDbSetting of RepoDb.Db2.BulkOperations. The given
        /// setting is always applied, even if DB2 was already initialized.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <param name="dbSetting">The setting to be used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseDb2(this GlobalConfiguration globalConfiguration,
            Db2DbSetting dbSetting)
        {
            if (dbSetting == null)
            {
                throw new System.ArgumentNullException(nameof(dbSetting));
            }

            Db2Bootstrap.InitializeInternal(dbSetting);
            return globalConfiguration;
        }
    }
}
