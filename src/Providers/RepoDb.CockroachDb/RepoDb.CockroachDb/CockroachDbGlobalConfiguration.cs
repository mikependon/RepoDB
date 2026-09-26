#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.CockroachDb;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="CockroachDbConnection"/> object.
    /// </summary>
    public static partial class CockroachDbGlobalConfiguration
    {
        /// <summary>
        /// Initializes all the necessary settings for CockroachDB.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseCockroachDb(this GlobalConfiguration globalConfiguration)
        {
            CockroachDbBootstrap.InitializeInternal();
            return globalConfiguration;
        }
    }
}
