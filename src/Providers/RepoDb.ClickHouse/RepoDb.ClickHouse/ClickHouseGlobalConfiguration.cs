#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using ClickHouse.Driver.ADO;
using RepoDb.DbSettings;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="ClickHouseConnection"/> object.
    /// </summary>
    /// <remarks>
    /// <see cref="ClickHouseConnection"/> here is <c>ClickHouse.Driver.ADO.ClickHouseConnection</c> - RepoDb no
    /// longer owns a subclass of its own.
    /// </remarks>
    public static partial class ClickHouseGlobalConfiguration
    {
        /// <summary>
        /// Initializes all the necessary settings for ClickHouse.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseClickHouse(this GlobalConfiguration globalConfiguration)
        {
            UseClickHouse(globalConfiguration, new ClickHouseDbSetting());
            return globalConfiguration;
        }

        /// <summary>
        /// Initializes all the necessary settings for ClickHouse.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <param name="isWaitForMutationsEnabled">A value indicating whether the internal mutations are enabled for the ClickHouse database.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseClickHouse(this GlobalConfiguration globalConfiguration,
            IDbSetting dbeStting)
        {
            ClickHouseBootstrap.InitializeInternal(dbeStting);
            return globalConfiguration;
        }
    }
}
