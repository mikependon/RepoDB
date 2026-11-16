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
    /// Configures the provider schema services.
    /// </summary>
    public static class ClickHouseSchemaGlobalConfiguration
    {
        #region Public Methods

        /// <summary>
        /// Initializes the ClickHouse settings first (see <c>UseClickHouse</c>), then registers the reader and <see cref="ClickHouseSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="ClickHouseConnection"/> type.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseClickHouseSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseClickHouse();
            SchemaComposerMapper.Add<ClickHouseConnection>(new ClickHouseSchemaComposer(),
                force: true);
            SchemaReaderMapper.Add<ClickHouseConnection>(new ClickHouseSchemaReader(),
                force: true);
            SchemaDialectMapper.Add(DbSettingMapper.Get<ClickHouseConnection>().Provider,
                ClickHouseSchemaDialect.Instance);
            return globalConfiguration;
        }
        #endregion
    }
}
