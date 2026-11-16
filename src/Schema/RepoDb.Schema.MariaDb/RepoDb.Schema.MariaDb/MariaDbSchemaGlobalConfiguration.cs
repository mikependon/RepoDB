#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using RepoDb.Connector.MariaDb;

namespace RepoDb.Schema.MariaDb
{
    /// <summary>
    /// Configures the provider schema services.
    /// </summary>
    public static class MariaDbSchemaGlobalConfiguration
    {
        #region Public Methods

        /// <summary>
        /// Initializes the MariaDB settings first (see <c>UseMariaDb</c>), then registers the reader and <see cref="MariaDbSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="MariaDbConnection"/> type.
        /// It is safe to call this method more than once.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseMariaDbSchema(this GlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseMariaDb();
            SchemaComposerMapper.Add<MariaDbConnection>(new MariaDbSchemaComposer(),
                force: true);
            SchemaReaderMapper.Add<MariaDbConnection>(new MariaDbSchemaReader(),
                force: true);
            SchemaDialectMapper.Add(DbSettingMapper.Get<MariaDbConnection>().Provider,
                MariaDbSchemaDialect.Instance);
            return globalConfiguration;
        }
        #endregion
    }
}
