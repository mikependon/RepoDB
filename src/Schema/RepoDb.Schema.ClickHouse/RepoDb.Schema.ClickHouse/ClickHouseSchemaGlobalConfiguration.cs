#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using ClickHouse.Driver;
using ClickHouse.Driver.ADO;

namespace RepoDb.Schema.ClickHouse
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings of the ClickHouse schema objects for the <see cref="ClickHouseConnection"/> object.
    /// </summary>
    public static class ClickHouseSchemaGlobalConfiguration
    {
        /// <summary>
        /// Initializes the ClickHouse settings first (see <c>UseClickHouse</c>), then registers the <see cref="ClickHouseSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="ClickHouseConnection"/> type.
        /// A <see cref="ClickHouseSchemaReader"/> owns the connection that it reads, so it is created with its connection instead of being registered here.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseClickHouseSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseClickHouse();
            SchemaComposerMapper.Add<ClickHouseConnection>(new ClickHouseSchemaComposer(), force: true);
            return globalConfiguration;
        }
    }
}
