#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using FirebirdSql.Data.FirebirdClient;
using RepoDb.DbSettings;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="FbConnection"/> object.
    /// </summary>
    public static partial class FirebirdConfiguration
    {
        /// <summary>
        /// Initializes all the necessary settings for Firebird.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseFirebird(this GlobalConfiguration globalConfiguration)
        {
            FirebirdBootstrap.InitializeInternal();
            return globalConfiguration;
        }

        /// <summary>
        /// Initializes all the necessary settings for Firebird, with the given setting. It accepts any class that inherits the
        /// <see cref="FirebirdDbSetting"/>, i.e. the FirebirdBulkOperationsDbSetting of RepoDb.Firebird.BulkOperations. The given
        /// setting is always applied, even if Firebird was already initialized.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <param name="dbSetting">The setting to be used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseFirebird(this GlobalConfiguration globalConfiguration,
            FirebirdDbSetting dbSetting)
        {
            if (dbSetting == null)
            {
                throw new System.ArgumentNullException(nameof(dbSetting));
            }

            FirebirdBootstrap.InitializeInternal(dbSetting);
            return globalConfiguration;
        }
    }
}
