#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.MariaDbConnector;

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
        public static GlobalConfiguration UseMariaDbConnector(
            this GlobalConfiguration globalConfiguration)
        {
            MariaDbBootstrap.InitializeInternal();
            return globalConfiguration;
        }
    }
}
