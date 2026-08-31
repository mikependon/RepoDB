#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Vertica.Data.VerticaClient;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="VerticaConnection"/> object.
    /// </summary>
    public static partial class VerticaConfiguration
    {
        /// <summary>
        /// Initializes all the necessary settings for Vertica.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <param name="useInvariantCulture">The flag that defines whether the invariant culture will be used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseVertica(
            this GlobalConfiguration globalConfiguration,
            bool useInvariantCulture = false)
        {
            VerticaBootstrap.InitializeInternal(useInvariantCulture);
            return globalConfiguration;
        }
    }
}
