#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Sap.Data.Hana;
using RepoDb.DbSettings;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="HanaConnection"/> object.
    /// </summary>
    public static partial class SapHanaConfiguration
    {
        /// <summary>
        /// Initializes all the necessary settings for SAP HANA.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseSapHana(this GlobalConfiguration globalConfiguration)
        {
            UseSapHana(globalConfiguration, new SapHanaDbSetting());
            return globalConfiguration;
        }

        /// <summary>
        /// Initializes all the necessary settings for SAP HANA.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <param name="dbSetting">The <see cref="IDbSetting"/> to be mapped against the <see cref="HanaConnection"/> object.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseSapHana(this GlobalConfiguration globalConfiguration,
            IDbSetting dbSetting)
        {
            SapHanaBootstrap.InitializeInternal(dbSetting);
            return globalConfiguration;
        }
    }
}
