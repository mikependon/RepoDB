#region Copyright Attributions

// Copyright (c) 2022 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using EnterpriseDB.EDBClient;

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
    }
}
